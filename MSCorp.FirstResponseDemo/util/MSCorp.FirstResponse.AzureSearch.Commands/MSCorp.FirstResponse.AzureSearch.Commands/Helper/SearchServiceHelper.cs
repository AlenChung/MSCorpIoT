using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Threading;
using MSCorp.FirstResponse.AzureSearch.Commands.Model;
using Newtonsoft.Json;
using RestSharp;

namespace MSCorp.FirstResponse.AzureSearch.Commands.Helper
{
    public class SearchServiceHelper
    {
        private readonly RestClient _client;
        private readonly string _searchApiKey;
        private readonly string _apiVersion;

        public SearchServiceHelper(RestClient client, string searchApiKey)
        {
            _client = client;
            _searchApiKey = searchApiKey;
            _apiVersion = "2015-02-28-Preview";
        }

        public void DeleteSearchIndex(string searchIndexname, Cmdlet cmdlet)
        {
            var request = new RestRequest($"indexes/{searchIndexname}?api-version={_apiVersion}");
            request.AddHeader("api-key", _searchApiKey);
            var response = _client.Delete(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                cmdlet.WriteVerbose($"Index {searchIndexname} does not exist ");
            }

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                cmdlet.WriteVerbose($"Index {searchIndexname} deleted");
            }
        }

        public void CreateIndex(string searchIndexname, string indexCreationJson, Cmdlet cmdlet)
        {
            var request = new RestRequest($"indexes?api-version={_apiVersion}", Method.POST);
            request.AddHeader("api-key", _searchApiKey);
            request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("application/json", indexCreationJson, ParameterType.RequestBody);
            var response = _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                cmdlet.ThrowTerminatingError(
                    new ErrorRecord(
                        new InvalidOperationException($"Unable to create index with response {response.Content}"),
                        response.StatusCode.ToString(), ErrorCategory.NotSpecified, null));
            }

            cmdlet.WriteVerbose($"Index {searchIndexname} created");
        }

        public void SeedData(string searchIndexname, ICollection<Person> persons, Cmdlet cmdlet)
        { 
              // batch through persons, azure search can only take 1000 at a time.
              var batchedData = SplitIntoBatches(persons, 1000);

            foreach (var personBatch in batchedData)
            {
                PushPeopleToSearchIndex(searchIndexname, personBatch, cmdlet);
            }

            // Wait a while for indexing to complete.
            Thread.Sleep(2000);
        }

        private void PushPeopleToSearchIndex(string searchIndexname, IList<Person> personList, Cmdlet cmdlet)
        {
            var request = new RestRequest($"indexes/{searchIndexname}/docs/index?api-version={_apiVersion}", Method.POST);
            request.AddHeader("api-key", _searchApiKey);
            request.AddHeader("Content-Type", "application/json");
            request.RequestFormat = DataFormat.Json;
            var personBatch = new PersonBatch {Value = personList.ToList()};
            var serializedObject = JsonConvert.SerializeObject(personBatch);
            request.AddParameter("application/json", serializedObject, ParameterType.RequestBody);
            var response = _client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                cmdlet.ThrowTerminatingError(
                     new ErrorRecord(
                         new InvalidOperationException($"Unable to populate  index with response {response.Content}"),
                         response.StatusCode.ToString(), ErrorCategory.NotSpecified, null));
            }

            cmdlet.WriteVerbose($"Index {searchIndexname} populated with a batch ({personList.Count})");
        }

        private static IEnumerable<IList<T>> SplitIntoBatches<T>(ICollection<T> dataToBatch, int batchSize)
        {
            var pageNumber = 0;
            var nextBatch = dataToBatch.Take(batchSize).ToList();
            while (nextBatch.Any())
            {
                yield return nextBatch;

                pageNumber++;

                nextBatch = dataToBatch
                    .Skip(pageNumber * batchSize)
                    .Take(batchSize)
                    .ToList();
            }
        }
    }
}
