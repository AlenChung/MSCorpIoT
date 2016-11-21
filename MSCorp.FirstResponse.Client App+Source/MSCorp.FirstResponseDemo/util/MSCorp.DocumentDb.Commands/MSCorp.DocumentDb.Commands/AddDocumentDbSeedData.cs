using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace MSCorp.DocumentDb.Commands
{
    [Cmdlet(VerbsCommon.Add, "DocumentDbSeedData")]
    public class AddDocumentDbSeedData : PSCmdlet
    {
        private const string DatabaseName = "FirstResponse";
        private const string IncidentCollectionName = "Tickets";

        [Parameter(Mandatory = true)]
        public string Url { get; set; }

        [Parameter(Mandatory = true)]
        public string Key { get; set; }

        [Parameter(Mandatory = true)]
        public string DataPath { get; set; }

        protected override void ProcessRecord()
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif

            //Make Synchronous for PSCmdlets to work correctly.
            var task = Task<int>.Factory.StartNew(() => RunAsync().Result);
            task.Wait();
            Console.WriteLine("Done Importing Data");
        }


        private async Task<int> RunAsync()
        {
            string data;
            using (var stream = new StreamReader(DataPath))
            {
                data = await stream.ReadToEndAsync();
            }

            List<dynamic> incidentList = await JsonConvert.DeserializeObjectAsync<List<dynamic>>(data);

            var client = new DocumentClient(new Uri(Url), Key);
            var db = await CreateDatabase(client, DatabaseName);
            var incidentCollection = await CreateDocumentCollection(client, db.Id, IncidentCollectionName);

            foreach (var document in incidentList)
            {
                await UploadDocument(client, db.Id, incidentCollection.Id, document);
            }
            Console.WriteLine("{0} Tickets Uploaded", incidentList.Count);

            return 0;
        }

        private static async Task<Database> CreateDatabase(DocumentClient client, string dbName)
        {
            // Check to verify a database with the id=FamilyRegistry does not exist
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == dbName).AsEnumerable().FirstOrDefault();
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(new Database { Id = dbName });
                Console.WriteLine(@"Database Created: {0}", database.Id);
            }
            else
            {
                Console.WriteLine(@"Database Found: {0}", database.Id);

                Console.WriteLine(@"Deleting Collections from database: {0}", database.Id);

                var collections = client.CreateDocumentCollectionQuery(database.CollectionsLink);
                foreach (var collection in collections)
                {
                    await client.DeleteDocumentCollectionAsync(collection.SelfLink);
                }
            }

            return database;
        }

        private static async Task<DocumentCollection> CreateDocumentCollection(DocumentClient client, string databaseId, string collectionId)
        {
            DocumentCollection documentCollection = client.CreateDocumentCollectionQuery("dbs/" + databaseId).Where(c => c.Id == collectionId).AsEnumerable().FirstOrDefault();

            // If the document collection does not exist, create a new collection
            if (documentCollection == null)
            {
                var collection = new DocumentCollection
                {
                    Id = collectionId,
                    IndexingPolicy = IndexingPolicyWithSpatialEnabled
                };
                documentCollection = await client.CreateDocumentCollectionAsync("dbs/" + databaseId, collection);

                // Write the new collection's id to the console
                Console.WriteLine(@"Document Collection Created: {0}", documentCollection.Id);
            }
            else
            {
                await ModifyCollectionWithSpatialIndexing(client, documentCollection, IndexingPolicyWithSpatialEnabled);
                Console.WriteLine(@"Document Collection Found: {0}", documentCollection.Id);
            }
            return documentCollection;
        }

        private static async Task UploadDocument(DocumentClient client, string databaseId, string collectionId, dynamic doc)
        {
            await client.CreateDocumentAsync("dbs/" + databaseId + "/colls/" + collectionId, doc);
        }

        private static async Task ModifyCollectionWithSpatialIndexing(DocumentClient client, DocumentCollection collection, IndexingPolicy indexingPolicy)
        {
            collection.IndexingPolicy = indexingPolicy;
            await client.ReplaceDocumentCollectionAsync(collection);

            long indexTransformationProgress = 0;

            while (indexTransformationProgress < 100)
            {
                ResourceResponse<DocumentCollection> response = await client.ReadDocumentCollectionAsync(collection.SelfLink);
                indexTransformationProgress = response.IndexTransformationProgress;

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        /// <summary>
        /// Gets an indexing policy with spatial enabled. You can also configure just certain paths for spatial indexing, e.g. Path = "/location/?"
        /// </summary>
        private static readonly IndexingPolicy IndexingPolicyWithSpatialEnabled = new IndexingPolicy
        {
            IncludedPaths = new System.Collections.ObjectModel.Collection<IncludedPath>()
            {
                new IncludedPath
                {
                    Path = "/*",
                    Indexes = new System.Collections.ObjectModel.Collection<Index>()
                    {
                        new SpatialIndex(DataType.Point),
                        new RangeIndex(DataType.Number) { Precision = -1 },
                        new RangeIndex(DataType.String) { Precision = -1 }
                    }
                }
            }
        };
    }
}
