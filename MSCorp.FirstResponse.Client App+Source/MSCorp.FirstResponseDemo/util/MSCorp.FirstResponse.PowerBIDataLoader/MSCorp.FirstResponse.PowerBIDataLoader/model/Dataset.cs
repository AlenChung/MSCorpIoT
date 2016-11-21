using System.Collections.Generic;

namespace MSCorp.FirstResponse.PowerBIDataLoader.model
{
    public class Dataset
    {
        public string name { get; set; }
        public List<Table> tables { get; set; }
    }
}