using Newtonsoft.Json;

namespace MSCorp.FirstResponse.PowerBIDataLoader.model
{
    public class FirstResponseRow
    {
        [JsonProperty(PropertyName = "Current High Priority Incidents")]
        public int PriorityIncidents { get; set; }

        [JsonProperty(PropertyName = "Current Average Response Time")]
        public int AverageResponseTime { get; set; }

        [JsonProperty(PropertyName = "Current Unassigned Incidents")]
        public int UnassignedTickets { get; set; }

        [JsonIgnore]
        public bool IsInitialSeed { get; set; }

    }
}