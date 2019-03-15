using ByrneLabs.TestoRoboto.HttpServices;
using ByrneLabs.TestoRoboto.HttpServices.Mutators;
using ByrneLabs.TestoRoboto.HttpServices.Mutators.Json;

namespace ByrneLabs.TestoRoboto.Jira
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var harSerializer = new HarSerializer();
            var collection = harSerializer.ReadFromFile("Jira.har");

            collection.RemoveDuplicateFingerprints(true);

            collection.AddFuzzedMessages(new Mutator[]
                {
                    //new HttpServices.Mutators.Headers.BlnsValueChanger(),
                    //new HttpServices.Mutators.Headers.HeaderDeleter(),
                    //new HttpServices.Mutators.Headers.RandomValueChanger(),
                    //new HttpServices.Mutators.Headers.BlnsValueChanger(),
                    //new HttpServices.Mutators.Headers.HeaderDeleter(),
                    //new HttpServices.Mutators.Headers.RandomValueChanger(),
                    //new HttpServices.Mutators.QueryStringParameters.BlnsValueChanger(),
                    //new HttpServices.Mutators.QueryStringParameters.ParameterDeleter(),
                    //new HttpServices.Mutators.QueryStringParameters.RandomValueChanger(),
                    //new ArrayGrower(),
                    //new ArrayShrinker(),
                    new BlnsValueChanger(),
                    //new PropertyAdder(),
                    //new PropertyRemover()
                }, false);

            var testRequest = new TestRequest();
            testRequest.Items.Add(collection);

            Dispatcher.Dispatch(testRequest);
        }
    }
}
