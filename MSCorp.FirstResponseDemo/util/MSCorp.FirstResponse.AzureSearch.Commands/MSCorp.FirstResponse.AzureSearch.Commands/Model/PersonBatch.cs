using System.Collections.Generic;
using Newtonsoft.Json;

namespace MSCorp.FirstResponse.AzureSearch.Commands.Model
{
    public class PersonBatch
    {
        [JsonProperty(PropertyName = "value")]
        public List<Person> Value { get; set; }
    }
}