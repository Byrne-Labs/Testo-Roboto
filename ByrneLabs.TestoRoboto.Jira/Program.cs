using System;
using System.Collections.Generic;
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
            testRequest.OnTheFlyMutators.Add(new HttpServices.Mutators.FormParameters.BlnsValueChanger());
            testRequest.OnTheFlyMutators.Add(new HttpServices.Mutators.Headers.BlnsValueChanger());
            testRequest.OnTheFlyMutators.Add(new HttpServices.Mutators.QueryStringParameters.BlnsValueChanger());
            testRequest.Items.Add(collection);
            testRequest.SessionData = GetSessionData();
            testRequest.LogDirectory = "server-errors";
            testRequest.ResponseErrorsToIgnore = new List<string>
            {
                @"Can not deserialize instance of java\.util\.HashSet out of VALUE_STRING token.*com\.atlassian\.greenhopper\.web\.rapid\.view\.CreatePresetsRequest\[\\""\w+\\""\]",
                @"Can not construct instance of boolean from String value .*: only \\""true\\"" or \\""false\\"" recognized",
                @"Can not deserialize instance of java\.util\.ArrayList out of VALUE_STRING token.*com\.atlassian\.webresource\.plugin\.rest\.Request\[\\""\w+\\""\]",
                @"java\.lang\.RuntimeException: Error parsing JQL",
                @"Can not deserialize instance of java\.util\.ArrayList out of VALUE_STRING token.*com\.atlassian\.jira\.quickedit\.rest\.api\.UserPreferences\[\\""\w+\\""]",
                @"Uploading the avatar has failed\. Please check that you are logged in and have sufficient permissions",
                @"Can not instantiate value of type \[map type; class java\.util\.LinkedHashMap, \[simple type, class java\.lang\.String\] -> \[simple type, class java\.lang\.String\]\] from JSON String"
            };

            Dispatcher.Dispatch(testRequest);
        }
    }
}
