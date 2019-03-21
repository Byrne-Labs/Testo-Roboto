using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ByrneLabs.TestoRoboto.Crawler.PageItems;
using OpenQA.Selenium;

namespace ByrneLabs.TestoRoboto.Crawler
{
    internal class Crawler : IDisposable
    {
        private readonly CrawlerSetup _crawlSetup;
        private ActionChain _currentActionChain;

        public Crawler(CrawlerSetup crawlSetup)
        {
            _crawlSetup = crawlSetup;
        }

        public void Crawl(string url)
        {
            InitializeWebDriver(url);
            _currentActionChain = new ActionChain();

            Crawl();
        }

        public void Crawl(ActionChain actionChain)
        {
            InitializeWebDriver(actionChain.Items.First().Url);

            foreach (var actionChainItem in actionChain.Items)
            {
                ExecuteActionChainItem(actionChainItem);
            }

            _currentActionChain = actionChain;

            Crawl();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _crawlSetup.WebDriver.Close();
                _crawlSetup.WebDriver.Dispose();
            }
        }

        private void Crawl()
        {
            var completedActionChain = _currentActionChain;
            while (_currentActionChain != null && !_currentActionChain.IsLooped && _currentActionChain.Items.Count <= _crawlSetup.MaximumChainLength)
            {
                var (discoveredActionChains, nextActionChainItem) = GetActionChainsForCurrentPage();
                _crawlSetup.CrawlManager.ReportDiscoveredActionChains(discoveredActionChains);

                _currentActionChain.Items.Add(nextActionChainItem);
                if (!_crawlSetup.CrawlManager.ShouldBeCrawled(_currentActionChain))
                {
                    _currentActionChain = null;
                    foreach (var discoveredActionChain in discoveredActionChains)
                    {
                        if (_crawlSetup.CrawlManager.ShouldBeCrawled(discoveredActionChain))
                        {
                            _currentActionChain = discoveredActionChain;
                            break;
                        }
                    }
                }

                if (_currentActionChain != null)
                {
                    ExecuteActionChainItem(_currentActionChain.Items.Last());
                    completedActionChain = _currentActionChain;
                }
            }

            _crawlSetup.CrawlManager.ReportCompletedActionChain(completedActionChain);
        }

        private void ExecuteActionChainItem(ActionChainItem actionChainItem)
        {
            foreach (var dataInputItem in actionChainItem.DataInputItems)
            {
                foreach (var dataInputHandler in _crawlSetup.DataInputHandlers)
                {
                    if (dataInputHandler.CanHandle(dataInputItem))
                    {
                        try
                        {
                            dataInputHandler.FillInput(_crawlSetup.WebDriver, dataInputItem);
                        }
                        catch (ElementNotVisibleException)
                        {
                            /*
                             * It is faster to try the action and catch the exception than it is to check the visibility of every element.
                             */
                        }
                        catch (ItemNotFoundException)
                        {
                            /*
                             * Ideally this should never happen but if it does, we can safely ignore it.  The worst case scenario is the input doesn't get set.
                             */
                        }

                        break;
                    }
                }
            }

            var actionHandled = false;
            foreach (var actionHandler in _crawlSetup.ActionHandlers)
            {
                if (actionHandler.CanHandle(actionChainItem.ChosenActionItem))
                {
                    try
                    {
                        actionHandler.ExecuteAction(_crawlSetup.WebDriver, actionChainItem.ChosenActionItem);
                    }
                    catch (ElementNotVisibleException)
                    {
                        /*
                         * It is faster to try the action and catch the exception than it is to check the visibility of every element.
                         */
                    }
                    catch (ItemNotFoundException)
                    {
                        /*
                         * Ideally this should never happen but if it does, we can safely ignore it and the action chain will loop out
                         */
                    }

                    actionHandled = true;
                    break;
                }
            }

            if (!actionHandled)
            {
                throw new InvalidOperationException("Unable to find action handler for action item");
            }

            try
            {
                _crawlSetup.WebDriver.SwitchTo().Alert().Accept();
            }
            catch (NoAlertPresentException)
            {
                /*
                 * Unfortunately, there is no way to determine if an alert is open without trying to do something like close it or click on a link below it.  This is intended
                 * to get rid of the "Changes you made may not be saved" alert that pops up when you navigate away from a page and have filled in some of the form fields.
                 */
            }
        }

        private (IEnumerable<ActionChain>, ActionChainItem) GetActionChainsForCurrentPage()
        {
            var availableActionItems = _crawlSetup.ActionHandlers.SelectMany(actionHandler => actionHandler.FindActions(_crawlSetup.WebDriver)).ToList();
            var dataInputItems = _crawlSetup.DataInputHandlers.SelectMany(actionHandler => actionHandler.FindDataInputs(_crawlSetup.WebDriver)).ToList();

            var actionChainItems = availableActionItems.Select(availableActionItem => new ActionChainItem(availableActionItems, dataInputItems, availableActionItem, _crawlSetup.WebDriver.Url)).ToList();

            var discoveredActionChains = new List<ActionChain>();
            if (actionChainItems.Count > 1)
            {
                foreach (var actionChainItem in actionChainItems.Skip(1))
                {
                    var discoveredActionChain = _currentActionChain.Clone();
                    discoveredActionChain.Items.Add(actionChainItem);
                    discoveredActionChains.Add(discoveredActionChain);
                }
            }

            return (discoveredActionChains, actionChainItems.FirstOrDefault());
        }

        private void InitializeWebDriver(string url)
        {
            _crawlSetup.WebDriver.Navigate().GoToUrl(url);
            Thread.Sleep(2000);
            _crawlSetup.WebDriver.Manage().Cookies.DeleteAllCookies();
            foreach (var cookie in _crawlSetup.SessionCookies)
            {
                _crawlSetup.WebDriver.Manage().Cookies.AddCookie(cookie);
            }

            _crawlSetup.WebDriver.Navigate().Refresh();
        }
    }
}
