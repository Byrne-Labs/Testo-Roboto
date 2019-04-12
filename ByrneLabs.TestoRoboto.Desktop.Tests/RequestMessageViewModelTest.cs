using System;
using System.Linq;
using ByrneLabs.TestoRoboto.Desktop.ViewModels;
using Xunit;

namespace ByrneLabs.TestoRoboto.Desktop.Tests
{
    public class RequestMessageViewModelTest
    {
        [Fact]
        public void TestAddQueryStringParameter()
        {
            var viewModel = new RequestMessageViewModel();
            Assert.Empty(viewModel.QueryStringParameters);
            viewModel.AddQueryStringParameter();
            Assert.NotEmpty(viewModel.QueryStringParameters);
        }

        [Fact]
        public void TestDeleteQueryStringParameter()
        {
            var viewModel = new RequestMessageViewModel();
            viewModel.AddQueryStringParameter();
            var parameter = viewModel.QueryStringParameters.Single();
            viewModel.AddQueryStringParameter();
            viewModel.SelectedQueryStringParameter = parameter;
            viewModel.DeleteSelectedQueryStringParameter();
            Assert.DoesNotContain(parameter, viewModel.QueryStringParameters);
            Assert.Single(viewModel.QueryStringParameters);
        }
    }
}
