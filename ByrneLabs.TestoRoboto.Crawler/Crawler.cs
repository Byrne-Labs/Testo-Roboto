using System;
using System.Collections.Generic;
using System.Linq;

namespace ByrneLabs.TestoRoboto.Crawler
{
    internal class Crawler : IDisposable
    {
        private readonly CrawlSetup _crawlSetup;
        private ActionChain _currentActionChain;

        public Crawler(CrawlSetup crawlSetup)
        {
            _crawlSetup = crawlSetup;
        }

        public void Crawl(string url)
        {
            _crawlSetup.WebDriver.Navigate().GoToUrl(url);

            _currentActionChain = new ActionChain();

            Crawl();
        }

        public void Crawl(ActionChain actionChain)
        {
            _crawlSetup.WebDriver.Navigate().GoToUrl(actionChain.Items.First().Url);

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
            while (_currentActionChain != null && !_currentActionChain.IsLooped && _currentActionChain.Items.Count <= _crawlSetup.MaxChainLength)
            {
                var discoveredActionChains = GetActionChainsForCurrentPage();
                _crawlSetup.CrawlManager.ReportDiscoveredActionChains(discoveredActionChains);

                _currentActionChain = null;
                foreach (var discoveredActionChain in discoveredActionChains)
                {
                    if (!discoveredActionChain.IsLooped && _crawlSetup.CrawlManager.ShouldBeCrawled(discoveredActionChain.Items.Last()))
                    {
                        _currentActionChain = discoveredActionChain;
                        break;
                    }
                }

                if (_currentActionChain != null)
                {
                    ExecuteActionChainItem(_currentActionChain.Items.Last());
                }
            }
        }

        private void ExecuteActionChainItem(ActionChainItem actionChainItem)
        {
            if (actionChainItem.ChosenActionItem.Tag != "a" || !actionChainItem.ChosenActionItem.Class.StartsWith("http", StringComparison.Ordinal))
            {
                foreach (var dataInputItem in actionChainItem.DataInputItems)
                {
                    foreach (var dataInputHandler in _crawlSetup.DataInputHandlers)
                    {
                        if (dataInputHandler.CanHandle(dataInputItem))
                        {
                            dataInputHandler.FillInput(_crawlSetup.WebDriver, dataInputItem);
                            break;
                        }
                    }
                }
            }

            var actionHandled = false;
            foreach (var actionHandler in _crawlSetup.ActionHandlers)
            {
                if (actionHandler.CanHandle(actionChainItem.ChosenActionItem))
                {
                    actionHandler.ExecuteAction(_crawlSetup.WebDriver, actionChainItem.ChosenActionItem);
                    actionHandled = true;
                    break;
                }
            }

            if (!actionHandled)
            {
                throw new InvalidOperationException("Unable to find action handler for action item");
            }
        }

        private IEnumerable<ActionChain> GetActionChainsForCurrentPage()
        {
            var availableActionItems = _crawlSetup.ActionHandlers.SelectMany(actionHandler => actionHandler.FindActions(_crawlSetup.WebDriver)).ToList();
            var dataInputItems = _crawlSetup.DataInputHandlers.SelectMany(actionHandler => actionHandler.FindDataInputs(_crawlSetup.WebDriver)).ToList();

            var actionChainItems = availableActionItems.Select(availableActionItem => new ActionChainItem(availableActionItems, dataInputItems, availableActionItem, _crawlSetup.WebDriver.Url)).ToList();

            var discoveredActionChains = new List<ActionChain>();
            foreach (var actionChainItem in actionChainItems)
            {
                var discoveredActionChain = _currentActionChain.Clone();
                discoveredActionChain.Items.Add(actionChainItem);
                discoveredActionChains.Add(discoveredActionChain);
            }

            return discoveredActionChains;
        }
    }
}
