using System.Collections.Generic;

namespace MSCorp.FirstResponse.PowerBIDataLoader.model
{
    public class Table
    {
        public string name { get; set; }
        public List<Column> columns { get; set; }
    }
}
