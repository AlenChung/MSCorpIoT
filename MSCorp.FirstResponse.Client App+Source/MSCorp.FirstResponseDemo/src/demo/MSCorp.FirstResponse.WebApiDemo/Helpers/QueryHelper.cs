using System;
using System.Collections.Generic;
using Microsoft.Azure.SqlDatabase.ElasticScale.Query;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using MSCorp.FirstResponse.WebApiDemo.Constants;
using MSCorp.FirstResponse.WebApiDemo.Models;


namespace MSCorp.FirstResponse.WebApiDemo.Helpers
{
    public static class QueryHelper
    {
        /// <summary>
        /// Executes a sql command on an elastic scale DB over all shards available in the provided shard map and returns incidents.
        /// </summary>
        public static IList<IncidentModel> ExecuteMultiShardQuery(string credentialsConnectionString, string commandText, params Shard[] shards)
        {
            if (shards == null)
            {
                throw new ArgumentNullException(nameof(shards));
            }
            if (credentialsConnectionString == null)
            {
                throw new ArgumentNullException(nameof(credentialsConnectionString));
            }
            if (commandText == null)
            {
                throw new ArgumentNullException(nameof(commandText));
            }

            // Get the shards to connect to
            List<IncidentModel> result = new List<IncidentModel>();
            // Create the multi-shard connection
            using (MultiShardConnection conn = new MultiShardConnection(shards, credentialsConnectionString))
            {
                // Create a simple command
                using (MultiShardCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = commandText;

                    // Append a column with the shard name where the row came from
                    cmd.ExecutionOptions = MultiShardExecutionOptions.IncludeShardNameColumn;

                    // Allow for partial results in case some shards do not respond in time
                    cmd.ExecutionPolicy = MultiShardExecutionPolicy.PartialResults;

                    // Allow the entire command to take up to 30 seconds
                    cmd.CommandTimeout = 30;

                    // Execute the command. 
                    // We do not need to specify retry logic because MultiShardDataReader will internally retry until the CommandTimeout expires.
                    using (MultiShardDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var incidentId = reader.GetFieldValue<int>(0);
                            var departmentType = reader.GetFieldValue<int>(1);
                            var title = reader.GetFieldValue<string>(2);
                            var desc = reader.GetFieldValue<string>(3);
                            var region = reader.GetFieldValue<int>(4);
                            var shardName = ExtractDatabaseName(reader.GetFieldValue<string>(5));
                            var incident = new IncidentModel
                            {
                                IncidentId = incidentId,
                                DepartmentType = departmentType,
                                ShardName = shardName,
                                Description = desc,
                                Title = title,
                                RegionId = region
                            };

                            result.Add(incident);
                        }
                    }
                }
            }
            return result;
        }

        private static string ExtractDatabaseName(string shardLocationString)
        {
            string[] pattern = { "[", "DataSource=", "Database=", "]" };
            string[] matches = shardLocationString.Split(pattern, StringSplitOptions.RemoveEmptyEntries);
            return matches[1];
        }
    }
}