using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using ByrneLabs.Serializer;
using ByrneLabs.TestoRoboto.Crawler;
using ByrneLabs.TestoRoboto.HttpServices;
using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;
using Cookie = OpenQA.Selenium.Cookie;

namespace ByrneLabs.TestoRoboto.Jira
{
    internal class Program
    {
        private static void CrawlJira()
        {
            var sessionData = GetSessionData();
            var crawlOptions = new CrawlOptions();
            crawlOptions.AllowedUrlPatterns.Add("^http://" + Environment.MachineName + ":8080");
            crawlOptions.MaximumChainLength = 12;
            crawlOptions.MaximumThreads = 12;
            crawlOptions.HeadlessBrowsing = true;
            crawlOptions.StartingUrls.Add("http://" + Environment.MachineName + ":8080");
            foreach (var cookie in sessionData.Cookies)
            {
                crawlOptions.SessionCookies.Add(new Cookie(cookie.Name, cookie.Value, cookie.Domain, cookie.Path, null));
            }

            var crawlManager = new CrawlManager(crawlOptions);
            crawlManager.Start();
            while (!crawlManager.Finished)
            {
                Thread.Sleep(60000);

                var requestMessageBytes = ByrneLabsSerializer.Serialize(crawlManager.DiscoveredRequestMessages);
                File.WriteAllBytes("jira crawl request messages.bls", requestMessageBytes);
            }
        }

        private static void FuzzTestHarFile()
        {
            var harSerializer = new HarSerializer();
            var collection = harSerializer.ReadFromFile("Jira4.har");

            collection.RemoveDuplicateFingerprints(true);

            var testRequest = new TestRequest();
            testRequest.OnTheFlyMutators.Add(new BlnsValueChanger());
            testRequest.OnTheFlyMutators.Add(new HttpServices.Mutators.FormParameters.BlnsValueChanger());
            testRequest.OnTheFlyMutators.Add(new HttpServices.Mutators.Headers.BlnsValueChanger());
            testRequest.OnTheFlyMutators.Add(new HttpServices.Mutators.QueryStringParameters.BlnsValueChanger());
            testRequest.Items.Add(collection);
            testRequest.SessionData = GetSessionData();
            testRequest.LogDirectory = "server-errors";
            testRequest.ResponseErrorsToIgnore.AddRange(new[]
            {
                @"Can not deserialize instance of java\.util\.HashSet out of VALUE_STRING token.*com\.atlassian\.greenhopper\.web\.rapid\.view\.CreatePresetsRequest\[\\""\w+\\""\]",
                @"Can not construct instance of boolean from String value .*: only \\""true\\"" or \\""false\\"" recognized",
                @"Can not deserialize instance of java\.util\.ArrayList out of VALUE_STRING token.*com\.atlassian\.webresource\.plugin\.rest\.Request\[\\""\w+\\""\]",
                @"java\.lang\.RuntimeException: Error parsing JQL",
                @"Can not deserialize instance of java\.util\.ArrayList out of VALUE_STRING token.*com\.atlassian\.jira\.quickedit\.rest\.api\.UserPreferences\[\\""\w+\\""]",
                @"Uploading the avatar has failed\. Please check that you are logged in and have sufficient permissions",
                @"Can not instantiate value of type \[map type; class java\.util\.LinkedHashMap, \[simple type, class java\.lang\.String\] -> \[simple type, class java\.lang\.String\]\] from JSON String",
                @"Can not deserialize instance of com.fasterxml.jackson.module.scala.deser.BuilderWrapper out of VALUE_STRING token",
                @"key &lt;\{\{expand\}\}&gt; doesn't match pattern",
                @"\{""errorMessages"":\[""Internal server error""\],""errors"":\{\}\}",
                @"java\.lang\.IllegalArgumentException: The scheme and the schemes id can not be null",
                @"java\.lang\.NullPointerException\r\n\tat com\.atlassian\.jira\.scheme\.AbstractEditScheme\.doExecute",
                @"java\.lang\.NullPointerException\r\n\tat com\.atlassian\.jira\.scheme\.AbstractEditScheme\.doDefault",
                @"java.lang.IllegalArgumentException: A resolution with the &#39;.+&#39; does not exist.\r\n\tat com.atlassian.jira.config.DefaultResolutionManager.checkResolutionExists",
                @"java.lang.NullPointerException\r\n\tat com.atlassian.jira.bc.config.DefaultStatusService.validateRemoveStatus",
                @"com.google.template.soy.tofu.SoyTofuException: In &#39;\w+&#39; tag, expression &quot;.+&quot; evaluates to undefined"
            });

            Dispatcher.Dispatch(testRequest);
        }

        private static SessionData GetSessionData()
        {
            var loginRequestMessage = new RequestMessage();
            loginRequestMessage.Uri = new Uri("http://" + Environment.MachineName + ":8080/login.jsp");
            loginRequestMessage.HttpMethod = HttpMethod.Post.ToString();
            loginRequestMessage.Headers.Add(new Header { Key = "Content-Type", Value = "application/x-www-form-urlencoded" });

            var loginBody = new FormUrlEncodedBody();
            loginBody.FormData.Add(new KeyValue { Key = "os_username", Value = "jonathan.byrne" });
            loginBody.FormData.Add(new KeyValue { Key = "os_password", Value = "Password1!" });
            loginRequestMessage.Body = loginBody;

            Dispatcher.Dispatch(loginRequestMessage);

            var sessionCookie = loginRequestMessage.ResponseMessages.Single().Cookies.Single(cookie => cookie.Name == "JSESSIONID");
            var xsrfCookie = loginRequestMessage.ResponseMessages.Single().Cookies.Single(cookie => cookie.Name == "atlassian.xsrf.token");

            var sessionData = new SessionData();
            sessionData.Cookies.Add(sessionCookie.Clone());
            sessionData.Cookies.Add(xsrfCookie.Clone());

            return sessionData;
        }

        private static void Main(string[] args)
        {
            CrawlJira();
        }
    }
}
