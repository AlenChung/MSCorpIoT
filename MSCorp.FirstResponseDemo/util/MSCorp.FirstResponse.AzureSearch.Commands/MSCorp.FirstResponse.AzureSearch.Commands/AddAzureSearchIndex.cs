using System;
using System.Management.Automation;
using MSCorp.FirstResponse.AzureSearch.Commands.Helper;
using RestSharp;

namespace MSCorp.FirstResponse.AzureSearch.Commands
{
    [Cmdlet(VerbsCommon.Add, "AzureSearchIndex")]
    public class AddAzureSearchIndex : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string SearchIndexName { get; set; }

        [Parameter(Mandatory = true)]
        public string SearchServiceApiKey { get; set; }

        [Parameter(Mandatory = true)]
        public string IndexCreationJsonFile { get; set; }

        [Parameter(Mandatory = true)]
        public string SearchServiceUri { get; set; }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose($"{nameof(SearchIndexName)} : {SearchIndexName}");
            WriteVerbose($"{nameof(SearchServiceApiKey)} : {SearchServiceApiKey}");
            WriteVerbose($"{nameof(IndexCreationJsonFile)} : {IndexCreationJsonFile}");
            WriteVerbose($"{nameof(SearchServiceUri)} : {SearchServiceUri}");
        }

        protected override void ProcessRecord()
        {
            try
            {
                var client = new RestClient(SearchServiceUri);
                var helper = new SearchServiceHelper(client, SearchServiceApiKey);
                helper.DeleteSearchIndex(SearchIndexName, this);
                var indexCreationJson = JsonFileHelper.ReadJsonFileToString(IndexCreationJsonFile);
                helper.CreateIndex(SearchIndexName, indexCreationJson, this);
            }
            catch (Exception e)
            {
                ThrowTerminatingError(new ErrorRecord(e, "101", ErrorCategory.CloseError, null));
            }
        }
    }
}
