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
        }

        public void Crawl(ActionChain actionChain)
        {
            foreach (var actionChainItem in actionChain.Items)
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

                ExecuteAction(actionChainItem.ChosenActionItem);
            }
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
                CrawlCurrentPage();

                ExecuteAction(_currentActionChain.Items.Last().ChosenActionItem);
            }
        }

        private void CrawlCurrentPage()
        {
            var currentActionChainItem = _currentActionChain.Items.Last();
            foreach (var dataInputItem in currentActionChainItem.DataInputItems)
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

            var actionChainItems = GetActionChainItemsForCurrentPage();
            var discoveredActionChains = new List<ActionChain>();
            foreach (var actionChainItem in actionChainItems)
            {
                var discoveredActionChain = _currentActionChain.Clone();
                discoveredActionChain.Items.Add(actionChainItem);
                discoveredActionChains.Add(discoveredActionChain);
            }

            _crawlSetup.CrawlManager.ReportDiscoveredActionChains(discoveredActionChains);

            _currentActionChain = null;
            foreach (var discoveredActionChain in discoveredActionChains)
            {
                if (_crawlSetup.CrawlManager.ShouldBeCrawled(discoveredActionChain.Items.Last()))
                {
                    _currentActionChain = discoveredActionChain;
                    break;
                }
            }
        }

        private void ExecuteAction(PageItem actionItem)
        {
            var actionHandled = false;
            foreach (var actionHandler in _crawlSetup.ActionHandlers)
            {
                if (actionHandler.CanHandle(actionItem))
                {
                    actionHandler.ExecuteAction(_crawlSetup.WebDriver, actionItem);
                    actionHandled = true;
                    break;
                }
            }

            if (!actionHandled)
            {
                throw new InvalidOperationException("Unable to find action handler for action item");
            }
        }

        private IEnumerable<ActionChainItem> GetActionChainItemsForCurrentPage()
        {
            var availableActionItems = _crawlSetup.ActionHandlers.SelectMany(actionHandler => actionHandler.FindActions(_crawlSetup.WebDriver)).ToList();
            var dataInputItems = _crawlSetup.DataInputHandlers.SelectMany(actionHandler => actionHandler.FindDataInputs(_crawlSetup.WebDriver)).ToList();

            return availableActionItems.Select(availableActionItem => new ActionChainItem(availableActionItems, dataInputItems, availableActionItem, _crawlSetup.WebDriver.Url)).ToList();
        }
    }
}
