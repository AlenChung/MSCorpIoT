using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using MSCorp.FirstResponse.WebApiDemo.Configuration;
using MSCorp.FirstResponse.WebApiDemo.Helpers;

namespace MSCorp.FirstResponse.WebApiDemo.Controllers
{
    /// <summary>
    /// Azure search endpoint for showcasing phonetic search.
    /// </summary>
    [AllowAnonymous, RoutePrefix("api/person")]
    public class PersonController : ApiController
    {
        /// <summary>
        /// Performs a search on an Azure Search index, the values searched for are set in the web config. Returns the result set from the search.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("search")]
        public async Task<DocumentSearchResult> Search([FromUri]string searchText)
        {
            var credentials = new SearchCredentials(AzureSearchConfig.SearchServiceApiKey);
            SearchIndexClient client = new SearchIndexClient(AzureSearchConfig.SearchServiceName
                , AzureSearchConfig.SearchIndexName, credentials);

            var callersGeoLocation = LocationHelper.GetCallerLocation();
            var scoringParam = new ScoringParameter(AzureSearchConfig.GeoScoringParameterName, 
                $"{callersGeoLocation.Longitude},{callersGeoLocation.Latitude}");

            var parameters = new SearchParameters
            {
                SearchMode = SearchMode.All,
                Facets = AzureSearchConfig.SearchIndexFacets,
                ScoringProfile = AzureSearchConfig.ScoringProfileName,
                ScoringParameters = new[] { scoringParam }
            };

            return await client.Documents.SearchAsync(searchText, parameters);
        }
    }
}
