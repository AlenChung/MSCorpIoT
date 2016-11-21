using System.Collections.Generic;

namespace MSCorp.FirstResponse.AzureSearch.Commands.Model
{
    public class GeoPoint
    {
        public GeoPoint(double latitude, double longitude)
        {
            coordinates= new List<double>();
            type = "Point";
            coordinates.Add(longitude);
            coordinates.Add(latitude);
        }

        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }
}