using System.Collections.Generic;
using MSCorp.FirstResponse.Client.Models;

namespace MSCorp.FirstResponse.Client.Services
{
    public static class SuspectService
    {
        private static readonly Dictionary<int, List<SuspectModel>> SuspectsIncidentsDictionary = new Dictionary<int, List<SuspectModel>>();
        public static void AddSuspectsToIncident(int incidentId, ICollection<SuspectModel> suspects)
        {
            if (!SuspectsIncidentsDictionary.ContainsKey(incidentId))
            {
                SuspectsIncidentsDictionary[incidentId] = new List<SuspectModel>();
            }

            SuspectsIncidentsDictionary[incidentId].AddRange(suspects);
        }

        public static IList<SuspectModel> GetSuspectsForIncident(int incidentId)
        {
            if (SuspectsIncidentsDictionary.ContainsKey(incidentId))
                return SuspectsIncidentsDictionary[incidentId];

            return new List<SuspectModel>();
        }
    }
}