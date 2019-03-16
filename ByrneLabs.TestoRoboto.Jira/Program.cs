using System;
using System.Linq;
using System.Net.Http;
using ByrneLabs.TestoRoboto.HttpServices;
using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;

namespace ByrneLabs.TestoRoboto.Jira
{
    internal class Program
    {
        private static SessionData GetSessionData()
        {
            var requestMessage = new RequestMessage();
            requestMessage.Uri = new Uri("http://localhost:8080/login.jsp");
            requestMessage.HttpMethod = HttpMethod.Post;
            requestMessage.Headers.Add(new Header { Key = "Content-Type", Value = "application/x-www-form-urlencoded" });

            var body = new FormUrlEncodedBody();
            body.FormData.Add(new KeyValue { Key = "os_username", Value = "jonathan.byrne" });
            body.FormData.Add(new KeyValue { Key = "os_password", Value = "Password1!" });
            requestMessage.Body = body;

            Dispatcher.Dispatch(requestMessage);

            var sessionCookie = requestMessage.ResponseMessages.Single().Cookies.Single(cookie => cookie.Name == "JSESSIONID");

            var sessionData = new SessionData();
            sessionData.Cookies.Add(sessionCookie.Clone());

            return sessionData;
        }

        private static void Main(string[] args)
        {
            var harSerializer = new HarSerializer();
            var collection = harSerializer.ReadFromFile("Jira.har");

            collection.RemoveDuplicateFingerprints(true);

            var testRequest = new TestRequest();
            testRequest.OnTheFlyMutators.Add(new BlnsValueChanger());
            testRequest.Items.Add(collection);
            testRequest.SessionData = GetSessionData();
            testRequest.LogDirectory = "server-errors";

            Dispatcher.Dispatch(testRequest);
        }
    }
}
