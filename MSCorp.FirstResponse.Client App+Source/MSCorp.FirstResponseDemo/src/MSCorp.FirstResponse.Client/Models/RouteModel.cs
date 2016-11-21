using System.Collections.Generic;
using Windows.Devices.Geolocation;

namespace MSCorp.FirstResponse.Client.Models
{
    public class RouteModel
    {
        public int Id { get; set; }
        public IList<BasicGeoposition> RoutePoints { get; set; }
        public IList<BasicGeoposition> FullRoute { get; set; }
    }
}