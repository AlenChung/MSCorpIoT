using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MSCorp.FirstResponse.PowerBIDataLoader.model;
using Newtonsoft.Json;
using RestSharp;

namespace MSCorp.FirstResponse.PowerBIDataLoader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Initializing with RestApi :'{Configuration.RestApiUrl}'");
            var client = new RestClient(Configuration.RestApiUrl);
            var datasetText = File.ReadAllText("dataset.json");
            var dataset = JsonConvert.DeserializeObject<Dataset>(datasetText);
            var accessToken = AccessToken();

            Console.WriteLine($"Looking for dataset with name '{dataset.name}'");
            string datasetId = LookupDataset(client, dataset.name, accessToken);

            if (string.IsNullOrEmpty(datasetId))
            {
                Console.WriteLine($"Creating dataset '{dataset.name}'");
                datasetId = CreateDataset(client, dataset, accessToken);
            }

            if (string.IsNullOrEmpty(datasetId))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Unable to find dataset '{dataset.name}' .... stopping");
                return;
            }

            var tableName = dataset.tables.First().name;
            var random = new Random(DateTime.Now.Millisecond);
            ClearOutRows(client, tableName, datasetId, accessToken);
            var builder = new FirstResponseRowBuilder(random);
            while (true)
            {
                try
                {
                    PopulateRows(client, tableName, datasetId, accessToken, builder);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }


                var seconds = Configuration.RefreshInSeconds;
                Console.WriteLine($"Waiting for {seconds} secs....");
                Thread.Sleep(TimeSpan.FromSeconds(seconds));
            }
        }

        private static void PopulateRows(RestClient client, string tableName, string datasetId, string accessToken, FirstResponseRowBuilder builder)
        {
            var request = new RestRequest($"datasets/{datasetId}/tables/{tableName}/rows");
            request.Method = Method.POST;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {accessToken}");
            var row = builder.Build();
            var collection = new RowCollection(row);
            var sollectionstring = JsonConvert.SerializeObject(collection);
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("application/json", sollectionstring, ParameterType.RequestBody);
            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (row.IsInitialSeed)
                {
                    Console.WriteLine($"Initial row populated with ART : {row.AverageResponseTime}, PI : {row.PriorityIncidents}, UT : {row.UnassignedTickets}");
                }

                Console.WriteLine($"Row delta created ART : {row.AverageResponseTime}, PI : {row.PriorityIncidents}, UT : {row.UnassignedTickets}");
                Console.WriteLine($"Total should be ART : {builder.AverageResponseTime}, PI : {builder.PriorityIncidents}, UT : {builder.UnassignedTickets}");

            }
            else
            {
                throw new InvalidOperationException($"Unable to populate rows, rsponse returned with ${response.StatusCode}");
            }
        }

        private static void ClearOutRows(RestClient client, string tableName, string datasetId, string token)
        {
            var request = new RestRequest($"datasets/{datasetId}/tables/{tableName}/rows");
            request.Method = Method.DELETE;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");
            var response = client.Execute<IdentityList>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine($"Row cleared from {tableName}");
            }
            else
            {
                throw new InvalidOperationException($"Unable to clear rows, rsponse returned with ${response.StatusCode}");
            }
        }

        private static string LookupDataset(RestClient client, string datasetName, string token)
        {
            var request = new RestRequest("datasets");
            request.Method = Method.GET;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");
            var response = client.Execute<IdentityList>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Identity identity = response.Data.value.FirstOrDefault(i => i.name == datasetName);
                if (identity != null)
                {
                    return identity.id;
                }
            }

            return string.Empty;
        }

        private static string CreateDataset(RestClient client, Dataset dataset, string token)
        {
            var request = new RestRequest("datasets");
            request.Method = Method.POST;
            
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddJsonBody(dataset);
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                return LookupDataset(client, dataset.name, token);
            }

            return string.Empty;
        }

        private static string AccessToken()
        {
            // https://dev.powerbi.com/apps - Native App.
            var clientId = Configuration.ClientId;
            var redirectUri = Configuration.RedirectUrl;
            string resourceUri = Configuration.ResourceUri;

            Console.WriteLine($"Authenticating with clientId : {clientId} and redirecturi {redirectUri}");
            //OAuth2 authority Uri
            string authorityUri = "https://login.windows.net/common/oauth2/authorize";
            AuthenticationContext authContext = new AuthenticationContext(authorityUri);
            var result = authContext.AcquireToken(resourceUri, clientId, new Uri(redirectUri), PromptBehavior.RefreshSession);
            string token = result.AccessToken;
            Console.WriteLine("Authenticated");

            return token;
        }
    }
}
