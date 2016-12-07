using System;
using System.Collections.Generic;
using System.Management.Automation;
using MSCorp.FirstResponse.AzureSearch.Commands.Helper;
using MSCorp.FirstResponse.AzureSearch.Commands.Model;
using Newtonsoft.Json;
using RestSharp;

namespace MSCorp.FirstResponse.AzureSearch.Commands
{
    [Cmdlet(VerbsCommon.Add, "AzureSearchIndexSeedData")]
    public class AddAzureSearchIndexSeedData : PSCmdlet
    {
        [Parameter(Mandatory = false)]
        public string SearchIndexName { get; set; }

        [Parameter(Mandatory = false)]
        public string SearchServiceApiKey { get; set; }

        [Parameter(Mandatory = true)]
        public string SearchServiceUri { get; set; }

        [Parameter(Mandatory = false)]
        public string PersonDataJsonFile { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose($"{nameof(SearchIndexName)} : {SearchIndexName}");
            WriteVerbose($"{nameof(SearchServiceApiKey)} : {SearchServiceApiKey}");
            WriteVerbose($"{nameof(PersonDataJsonFile)} : {PersonDataJsonFile}");
            WriteVerbose($"{nameof(SearchServiceUri)} : {SearchServiceUri}");
        }

        protected override void ProcessRecord()
        {
            try
            {
                WriteVerbose($"Seeding data to search index: {SearchIndexName}");
                var client = new RestClient(SearchServiceUri);
                var helper = new SearchServiceHelper(client, SearchServiceApiKey);
                var documents = JsonConvert.DeserializeObject<List<Person>>(JsonFileHelper.ReadJsonFileToString(PersonDataJsonFile));
                helper.SeedData(SearchIndexName, documents, this);
            }
            catch (Exception e)
            {
                ThrowTerminatingError(new ErrorRecord(e, "102", ErrorCategory.CloseError, null));
            }
        }
    }
}