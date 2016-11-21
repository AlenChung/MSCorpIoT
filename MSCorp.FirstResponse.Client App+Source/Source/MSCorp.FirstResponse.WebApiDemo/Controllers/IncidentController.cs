using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.Azure.SqlDatabase.ElasticScale.ShardManagement;
using MSCorp.FirstResponse.WebApiDemo.Configuration;
using MSCorp.FirstResponse.WebApiDemo.Constants;
using MSCorp.FirstResponse.WebApiDemo.Helpers;
using MSCorp.FirstResponse.WebApiDemo.Models;

namespace MSCorp.FirstResponse.WebApiDemo.Controllers
{
    /// <summary>
    /// This controller's endpoints access incidents from SQL Elastic scale DB shards.
    /// </summary>
    [AllowAnonymous, RoutePrefix("api/incidents")]
    public class IncidentController : ApiController
    {
        readonly Lazy<RangeShardMap<int>> _shardMap = new Lazy<RangeShardMap<int>>(GetShardMap);

        /// <summary>
        /// Returns all incidents stored accross all shards
        /// </summary>
        [HttpGet, Route]
        public IList<IncidentModel> GetAllIncidents()
        {
            string connectionString = ShardManagmentConfig.GetCredentialsConnectionString();
            Shard[] shards = _shardMap.Value.GetShards().ToArray();

            return QueryHelper.ExecuteMultiShardQuery(connectionString, IncidentQuery, shards);
        }

        /// <summary>
        /// Returns only ambulance incidents, stored on the ambulance incidents shard
        /// </summary>
        [HttpGet, Route("ambulance")]
        public IList<IncidentModel> GetAmbulanceIncidents()
        {
            string connectionString = ShardManagmentConfig.GetCredentialsConnectionString();
            Shard shard = _shardMap.Value.GetMappingForKey((int)DepartmentType.Ambulance).Shard;

            return QueryHelper.ExecuteMultiShardQuery(connectionString, IncidentQuery, shard);
        }

        /// <summary>
        /// Returns only police incidents, stored on the police incidents shard
        /// </summary>
        [HttpGet, Route("police")]
        public IList<IncidentModel> GetPoliceIncidents()
        {
            string connectionString = ShardManagmentConfig.GetCredentialsConnectionString();
            Shard shard = _shardMap.Value.GetMappingForKey((int)DepartmentType.Police).Shard;

            return QueryHelper.ExecuteMultiShardQuery(connectionString, IncidentQuery, shard);
        }

        /// <summary>
        /// Returns only fire incidents, stored on the fire incidents shard
        /// </summary>
        [HttpGet, Route("fire")]
        public IList<IncidentModel> GetFireIncidents()
        {
            string connectionString = ShardManagmentConfig.GetCredentialsConnectionString();
            Shard shard = _shardMap.Value.GetMappingForKey((int)DepartmentType.Fire).Shard;

            return QueryHelper.ExecuteMultiShardQuery(connectionString, IncidentQuery, shard);
        }

        private static RangeShardMap<int> GetShardMap()
        {
            var shardMapManager = ShardManagementUtils.TryGetShardMapManager(
                ShardManagmentConfig.ShardMapManagerServerName
                , ShardManagmentConfig.ShardMapDatabase
            );
            return shardMapManager.GetRangeShardMap<int>(ShardManagmentConfig.ShardMapName);
        }

        private static string IncidentQuery => @"
                    SELECT 
                        i.IncidentId, 
                        i.DepartmentType,
                        i.Title,
                        i.Description,
                        i.RegionId,
                        i.Latitude,
                        i.Longitude
                    FROM 
                        dbo.Incidents AS i
                ";
    }
}
