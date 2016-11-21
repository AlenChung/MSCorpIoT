using System;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;

namespace MSCorp.FirstResponse.Client.Models
{
    public class TicketModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Violation { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Geopoint Location => new Geopoint(new BasicGeoposition { Latitude = Latitude, Longitude = Longitude });

        public MapIcon ToMapIcon()
        {
            return new MapIcon
            {
                Location = Location,
                NormalizedAnchorPoint = new Point(0.5, 1),
                ZIndex = 998,
                Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/pins/pin_car.png"))
            };
        }
    }
}
