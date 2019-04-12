using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ByrneLabs.Commons;
using ByrneLabs.TestoRoboto.Crawler.PageItems;
using ByrneLabs.TestoRoboto.HttpServices;
//using JetBrains.Annotations;
using MessagePack;
using MessagePack.Resolvers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using Cookie = ByrneLabs.TestoRoboto.HttpServices.Cookie;

namespace ByrneLabs.TestoRoboto.Crawler
{
    
    public class CrawlManager
    {
        private class FingerprintRequestMessageComparer : IEqualityComparer<RequestMessage>
        {
            public bool Equals(RequestMessage x, RequestMessage y) => x?.Fingerprint == y?.Fingerprint;

            public int GetHashCode(RequestMessage obj) => obj.Fingerprint.GetHashCode();
        }

        private static readonly object _logFileLock = new object();
        private static int _nextProxyPort = 49152;
        private List<ActionChain> _actionChainsToCrawl = new List<ActionChain>();
        private CancellationTokenSource _cancellationTokenSource;
        private IList<ActionChainItem> _crawledActionChainItems = new List<ActionChainItem>();
        private CrawlOptions _crawlOptions;
        private Task _crawlTask;
        private bool _initialPagesCrawled;
        private int _proxyPort;
        private ConcurrentBag<RequestMessage> _requestMessages = new ConcurrentBag<RequestMessage>();

        public CrawlManager(CrawlOptions crawlOptions) : this()
        {
            /*
             * We clone it to make sure it doesn't get changed during the middle of the crawl being executed.
             */
            _crawlOptions = crawlOptions.Clone();
        }

        private CrawlManager()
        {
            SetupProxyServer();
        }

        public IEnumerable<RequestMessage> DiscoveredRequestMessages => _requestMessages.Distinct(new FingerprintRequestMessageComparer()).ToList();

        public bool Finished { get; private set; }

        public static CrawlManager ResumeFromFile(string fileName)
        {
            var fileBytes = File.ReadAllBytes(fileName);

            var resumedCrawlManager = new CrawlManager();

            using (var memoryStream = new MemoryStream(fileBytes))
            using (var binaryReader = new BinaryReader(memoryStream))
            {
                resumedCrawlManager._actionChainsToCrawl = new List<ActionChain>(ReadFromSerializationBytes<List<ActionChain>>(binaryReader));
                resumedCrawlManager._crawledActionChainItems = ReadFromSerializationBytes<List<ActionChainItem>>(binaryReader);
                resumedCrawlManager._crawlOptions = ReadFromSerializationBytes<CrawlOptions>(binaryReader);
                resumedCrawlManager._requestMessages = new ConcurrentBag<RequestMessage>(ReadFromSerializationBytes<List<RequestMessage>>(binaryReader));
                resumedCrawlManager._initialPagesCrawled = binaryReader.ReadBoolean();
            }

            return resumedCrawlManager;
        }

        private static T ReadFromSerializationBytes<T>(BinaryReader binaryReader)
        {
            var byteLength = binaryReader.ReadInt32();
            var bytes = binaryReader.ReadBytes(byteLength);
            var obj = MessagePackSerializer.Deserialize<T>(bytes);

            return obj;
        }

        private static void WriteToSerializationBytes(BinaryWriter binaryWriter, object obj)
        {
            var bytes = MessagePackSerializer.Serialize(obj);
            binaryWriter.Write(bytes.Length);
            binaryWriter.Write(bytes);
        }

        public void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _crawlTask = new Task(() =>
            {
                var parallelOptions = new ParallelOptions();
                parallelOptions.CancellationToken = _cancellationTokenSource.Token;
                parallelOptions.MaxDegreeOfParallelism = _crawlOptions.MaximumThreads;

                if (!_initialPagesCrawled)
                {
                    Parallel.ForEach(_crawlOptions.StartingUrls, parallelOptions, url =>
                    {
                        try
                        {
                            using (var initialCrawler = new Crawler(GetCrawlerSetup(_crawlOptions)))
                            {
                                initialCrawler.Crawl(url);
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }

                        parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                    });
                }

                _initialPagesCrawled = true;
                BetterParallel.While(parallelOptions, () => _actionChainsToCrawl.Any(), () =>
                {
                    ActionChain actionChainToCrawl;
                    lock (_actionChainsToCrawl)
                    {
                        actionChainToCrawl = _actionChainsToCrawl.OrderBy(actionChain => actionChain.Priority).ThenBy(x => BetterRandom.Next()).FirstOrDefault();
                        if (actionChainToCrawl != null)
                        {
                            _actionChainsToCrawl.Remove(actionChainToCrawl);
                        }
                    }

                    if (actionChainToCrawl != null)
                    {
                        try
                        {
                            if (ShouldBeCrawled(actionChainToCrawl))
                            {
                                using (var crawler = new Crawler(GetCrawlerSetup(_crawlOptions)))
                                {
                                    crawler.Crawl(actionChainToCrawl);
                                }
                            }
                        }
                        catch (WebDriverException exception)
                        {
                            actionChainToCrawl.Exception = exception;
                            if (Regex.IsMatch(exception.Message, "stale element reference: element is not attached to the page document"))
                            {
                                actionChainToCrawl.TerminationReason = "Stale Element Reference";
                            }
                            else if (Regex.IsMatch(exception.Message, "The HTTP request to the remote WebDriver server for URL"))
                            {
                                actionChainToCrawl.TerminationReason = "WebDriver Server Not Responding";
                            }
                            else
                            {
                                actionChainToCrawl.TerminationReason = "Selenium Exception - " + exception.Message;
                            }

                            ReportCompletedActionChain(actionChainToCrawl);
                        }
                        catch (Exception exception)
                        {
                            actionChainToCrawl.Exception = exception;
                            actionChainToCrawl.TerminationReason = "Exception - " + exception.Message;
                            ReportCompletedActionChain(actionChainToCrawl);
                        }
                    }

                    parallelOptions.CancellationToken.ThrowIfCancellationRequested();
                });
                Finished = true;
            }, _cancellationTokenSource.Token);
            _crawlTask.Start();
        }

        public void Stop(string saveToFileName)
        {
            Stop();

            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                WriteToSerializationBytes(binaryWriter, _actionChainsToCrawl);
                WriteToSerializationBytes(binaryWriter, _crawledActionChainItems.ToList());
                WriteToSerializationBytes(binaryWriter, _crawlOptions);
                WriteToSerializationBytes(binaryWriter, _requestMessages.ToList());
                binaryWriter.Write(_initialPagesCrawled);

                File.WriteAllBytes(saveToFileName, memoryStream.ToArray());
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel(false);
            while (!_crawlTask.IsCompleted)
            {
                Thread.Sleep(100);
            }
        }

        internal void ReportCompletedActionChain(ActionChain actionChain)
        {
            lock (_logFileLock)
            {
                File.AppendAllText("completed action chains.txt", $"{DateTime.Now.ToString(CultureInfo.InvariantCulture)}\t{actionChain.TerminationReason}\t{actionChain.Items.Count}\t{actionChain}\r\n");
            }
        }

        internal void ReportDiscoveredActionChains(IEnumerable<ActionChain> discoveredActionChains)
        {
            lock (_actionChainsToCrawl)
            {
                foreach (var discoveredActionChain in discoveredActionChains)
                {
                    if (!discoveredActionChain.Items.Last().Crawled && !discoveredActionChain.IsLooped)
                    {
                        _actionChainsToCrawl.Add(discoveredActionChain);
                    }
                }
            }
        }

        internal bool ShouldBeCrawled(ActionChain actionChain)
        {
            lock (_crawledActionChainItems)
            {
                if (actionChain.IsLooped)
                {
                    actionChain.TerminationReason = "Looped";
                    return false;
                }

                var lastActionChainItem = actionChain.Items.Last();
                if (lastActionChainItem == null)
                {
                    Debugger.Break();
                }

                if (_crawledActionChainItems.Contains(lastActionChainItem))
                {
                    actionChain.TerminationReason = "Already Crawled";
                    return false;
                }

                if (!_crawlOptions.AllowedUrlPatterns.Any(allowedUrlPattern => Regex.IsMatch(lastActionChainItem.Url, allowedUrlPattern, RegexOptions.IgnoreCase)))
                {
                    actionChain.TerminationReason = "Banned URL Pattern";
                    return false;
                }

                if (lastActionChainItem.Crawled)
                {
                    actionChain.TerminationReason = "No More Actions";
                    return false;
                }

                _crawledActionChainItems.Add(lastActionChainItem);
                lastActionChainItem.Crawled = true;
                lock (_logFileLock)
                {
                    File.AppendAllText("crawled actions.txt", $"{DateTime.Now.ToString(CultureInfo.InvariantCulture)}\t{lastActionChainItem}\r\n");
                }

                return true;
            }
        }

        private CrawlerSetup GetCrawlerSetup(CrawlOptions crawlOptions)
        {
            var crawlerSetup = new CrawlerSetup();
            crawlerSetup.ActionHandlers.Add(new AnchorHandler());
            crawlerSetup.ActionHandlers.Add(new ButtonHandler());

            crawlerSetup.DataInputHandlers.Add(new DateInputHandler());
            crawlerSetup.DataInputHandlers.Add(new DateTimeInputHandler());
            crawlerSetup.DataInputHandlers.Add(new EmailInputHandler());
            crawlerSetup.DataInputHandlers.Add(new MonthInputHandler());
            crawlerSetup.DataInputHandlers.Add(new NumberInputHandler());
            crawlerSetup.DataInputHandlers.Add(new TelephoneInputHandler());
            crawlerSetup.DataInputHandlers.Add(new TextInputHandler());
            crawlerSetup.DataInputHandlers.Add(new TimeInputHandler());
            crawlerSetup.DataInputHandlers.Add(new UrlInputHandler());
            crawlerSetup.DataInputHandlers.Add(new WeekInputHandler());

            crawlerSetup.CrawlManager = this;

            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.SuppressInitialDiagnosticInformation = true;

            var chromeOptions = new ChromeOptions();

            if (crawlOptions.DisableImageDownloading)
            {
                chromeOptions.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            }

            if (crawlOptions.HeadlessBrowsing)
            {
                chromeOptions.AddArgument("headless");
            }

            chromeOptions.AcceptInsecureCertificates = true;
            chromeOptions.UnhandledPromptBehavior = UnhandledPromptBehavior.Accept;
            chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.Severe);
            chromeOptions.SetLoggingPreference(LogType.Client, LogLevel.Severe);
            chromeOptions.SetLoggingPreference(LogType.Driver, LogLevel.Severe);
            chromeOptions.SetLoggingPreference(LogType.Profiler, LogLevel.Severe);
            chromeOptions.SetLoggingPreference(LogType.Server, LogLevel.Severe);
            chromeOptions.AddArgument("log-level=3");
            chromeOptions.AddArgument("silent");
            chromeOptions.Proxy = new Proxy { HttpProxy = "localhost:" + _proxyPort, SslProxy = "localhost:" + _proxyPort };
            crawlerSetup.WebDriver = new ChromeDriver(driverService, chromeOptions);

            foreach (var cookie in crawlOptions.SessionCookies)
            {
                crawlerSetup.SessionCookies.Add(cookie);
            }

            crawlerSetup.MaximumChainLength = crawlOptions.MaximumChainLength;
            return crawlerSetup;
        }

        private async Task ProxyServer_BeforeRequest(object sender, SessionEventArgs e)
        {
            await Task.Run(() =>
            {
                var requestMessage = new RequestMessage();
                requestMessage.Uri = new Uri(e.HttpClient.Request.Url);
                foreach (var tiHeader in e.HttpClient.Request.Headers.Where(th => th.Name != "Cookie"))
                {
                    var header = new Header();
                    header.Key = tiHeader.Name;
                    header.Value = tiHeader.Value;
                    requestMessage.Headers.Add(header);
                }

                var cookies = e.HttpClient.Request.Headers.HeaderExists("Cookie") ? e.HttpClient.Request.Headers.GetFirstHeader("Cookie").Value : string.Empty;
                foreach (var cookie in cookies.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var name = cookie.SubstringBeforeLast("=");
                    var value = cookie.SubstringAfterLast("=");

                    requestMessage.Cookies.Add(new Cookie { Name = name, Value = value });
                }

                requestMessage.Encoding = e.HttpClient.Request.Encoding.WebName;

                var contentType = e.HttpClient.Request.Headers.HeaderExists("Content-Type") ? e.HttpClient.Request.Headers.GetFirstHeader("Content-Type").Value : string.Empty;

                if (!e.HttpClient.Request.HasBody)
                {
                    requestMessage.Body = new NoBody();
                }
                else
                {
                    var bodyText = e.GetRequestBodyAsString().Result;
                    if (contentType == "application/x-www-form-urlencoded")
                    {
                        requestMessage.Body = FormUrlEncodedBody.GetFromBodyText(bodyText);
                    }
                    else if (contentType == "multipart/form-data")
                    {
                        requestMessage.Body = FormDataBody.GetFromBodyText(bodyText);
                    }
                    else
                    {
                        requestMessage.Body = RawBody.GetFromBodyText(bodyText);
                    }
                }

                requestMessage.HttpMethod = e.HttpClient.Request.Method;

                _requestMessages.Add(requestMessage);
            });
        }

        private async Task ProxyServer_BeforeResponse(object sender, SessionEventArgs e)
        {
            await Task.Run(() => { });
        }

        private void SetupProxyServer()
        {
            var proxyServer = new ProxyServer();
            proxyServer.CertificateManager.TrustRootCertificate(true);
            proxyServer.BeforeRequest += ProxyServer_BeforeRequest;
            proxyServer.BeforeResponse += ProxyServer_BeforeResponse;
            _proxyPort = _nextProxyPort++;
            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, _proxyPort);
            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.Start();
        }
    }
}
