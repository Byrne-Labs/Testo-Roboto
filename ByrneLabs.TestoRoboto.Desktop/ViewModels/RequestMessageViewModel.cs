﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ByrneLabs.Commons;
using ByrneLabs.Commons.Collections;
using ByrneLabs.Commons.Presentation.Wpf;

namespace ByrneLabs.TestoRoboto.Desktop.ViewModels
{
    public class RequestMessageViewModel : RequestMessageHierarchyItemViewModel
    {
        private Uri _url;
        private bool _urlChanging;

        public RequestMessageViewModel()
        {
            AddQueryStringParameterCommand = new RelayCommand(param => OnAddQueryStringParameter());
            DeleteSelectedQueryStringParameterCommand = new RelayCommand(param => OnDeleteSelectedQueryStringParameter(), param => CanDeleteSelectedQueryStringParameter());
            AddHeaderCommand = new RelayCommand(param => OnAddHeader());
            DeleteSelectedHeaderCommand = new RelayCommand(param => OnDeleteSelectedHeader(), param => CanDeleteSelectedHeader());
            AddCookieCommand = new RelayCommand(param => OnAddCookie());
            DeleteSelectedCookieCommand = new RelayCommand(param => OnDeleteSelectedCookie(), param => CanDeleteSelectedCookie());
            QueryStringParameters.CollectionChanged += (sender, args) =>
            {
                if (_url != null && !_urlChanging)
                {
                    var uriBuilder = new StringBuilder(Url.ToString().Contains("?") ? Url.ToString().SubstringBeforeLast("?") : Url.ToString()).Append('?');
                    foreach (var queryStringParameter in QueryStringParameters)
                    {
                        uriBuilder.Append(queryStringParameter.Key).Append('=').Append(queryStringParameter.UriEncodedValue).Append('&');
                    }

                    uriBuilder.Remove(uriBuilder.Length - 1, 1);

                    var newUrl = new Uri(uriBuilder.Length <= 2083 ? uriBuilder.ToString() : uriBuilder.ToString().Substring(0, 2083));
                    if (newUrl.ToString() != _url.ToString())
                    {
                        _url = newUrl;
                        OnPropertyChanged(nameof(Url));
                    }
                }
            };
        }

        public RelayCommand AddCookieCommand { get; }

        public RelayCommand AddHeaderCommand { get; }

        public RelayCommand AddQueryStringParameterCommand { get; }

        public IEnumerable<BodyViewModel> BodyTypes { get; } = new BodyViewModel[] { new NoBodyViewModel(), new FormDataBodyViewModel(), new RawBodyViewModel(), new UrlEncodedBodyViewModel() };

        public BodyViewModel BodyViewModel { get; set; }

        public ObservableCollection<CookieViewModel> Cookies { get; } = new FullyObservableCollection<CookieViewModel>();

        public RelayCommand DeleteSelectedCookieCommand { get; }

        public RelayCommand DeleteSelectedHeaderCommand { get; }

        public RelayCommand DeleteSelectedQueryStringParameterCommand { get; }

        public ObservableCollection<HeaderViewModel> Headers { get; } = new FullyObservableCollection<HeaderViewModel>();

        public string HttpMethod { get; set; } = "POST";

        public IEnumerable<string> HttpMethods { get; } = new[] { "GET", "POST", "PUT", "PATCH", "DELETE", "COPY", "HEAD", "OPTIONS", "LINK", "UNLINK", "PURGE", "LOCK", "UNLOCK", "PROPFIND", "VIEW" };

        public ObservableCollection<QueryStringParameterViewModel> QueryStringParameters { get; } = new FullyObservableCollection<QueryStringParameterViewModel>();

        public CookieViewModel SelectedCookie { get; set; }

        public HeaderViewModel SelectedHeader { get; set; }

        public QueryStringParameterViewModel SelectedQueryStringParameter { get; set; }

        public Uri Url
        {
            get => _url;
            set
            {
                if (_url != value)
                {
                    _urlChanging = true;
                    var queryStringParameters = new List<QueryStringParameterViewModel>();
                    _url = value;
                    if (_url.Query != string.Empty)
                    {
                        foreach (var parameter in _url.Query.SubstringAfterLast("?").Split('&').Where(p => p != string.Empty))
                        {
                            var queryStringParameter = new QueryStringParameterViewModel();
                            queryStringParameter.Key = parameter.SubstringBeforeLast("=");
                            queryStringParameter.Value = parameter.SubstringAfterLast("=");
                            queryStringParameters.Add(queryStringParameter);
                        }
                    }

                    foreach (var newQueryStringParameter in queryStringParameters.Where(qsp => !QueryStringParameters.Any(qsp2 => qsp.Key == qsp2.Key && qsp.Value == qsp2.Value)))
                    {
                        if (QueryStringParameters.Count(qsp => qsp.Key == newQueryStringParameter.Key) == 1)
                        {
                            QueryStringParameters.Single(qsp => qsp.Key == newQueryStringParameter.Key).Value = newQueryStringParameter.Value;
                        }
                        else
                        {
                            QueryStringParameters.Add(newQueryStringParameter);
                        }
                    }

                    var queryStringParametersToRemove = new List<QueryStringParameterViewModel>();
                    foreach (var existingQueryStringParameter in QueryStringParameters)
                    {
                        if (!queryStringParameters.Exists(qsp => qsp.Key == existingQueryStringParameter.Key && qsp.Value == existingQueryStringParameter.Value))
                        {
                            queryStringParametersToRemove.Add(existingQueryStringParameter);
                        }
                    }

                    foreach (var queryStringParameterToRemove in queryStringParametersToRemove)
                    {
                        QueryStringParameters.Remove(queryStringParameterToRemove);
                    }

                    _urlChanging = false;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }

        public bool CanDeleteSelectedCookie() => SelectedCookie != null;

        public bool CanDeleteSelectedHeader() => SelectedHeader != null;

        public bool CanDeleteSelectedQueryStringParameter() => SelectedQueryStringParameter != null;

        public void OnAddCookie()
        {
            Cookies.Add(new CookieViewModel());
        }

        public void OnAddHeader()
        {
            Headers.Add(new HeaderViewModel());
        }

        public void OnAddQueryStringParameter()
        {
            QueryStringParameters.Add(new QueryStringParameterViewModel());
        }

        public void OnDeleteSelectedCookie()
        {
            Cookies.Remove(SelectedCookie);
        }

        public void OnDeleteSelectedHeader()
        {
            Headers.Remove(SelectedHeader);
        }

        public void OnDeleteSelectedQueryStringParameter()
        {
            QueryStringParameters.Remove(SelectedQueryStringParameter);
        }
    }
}
