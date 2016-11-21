using System.Collections.Generic;

namespace MSCorp.FirstResponse.PowerBIDataLoader.model
{
    public class RowCollection
    {
        public RowCollection(FirstResponseRow row)
        {
            rows = new List<FirstResponseRow>();
            rows.Add(row);
        }

        public List<FirstResponseRow> rows { get; set; }
    }
}