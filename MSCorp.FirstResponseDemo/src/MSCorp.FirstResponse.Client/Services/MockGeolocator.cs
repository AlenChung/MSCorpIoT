using System;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace MSCorp.FirstResponse.Client.Services
{
    public class MockGeolocator
    {
        private Geopoint _location;

        public Geopoint GetGeoposition()
        {
            return _location ?? (_location = Configuration.StartLocation);
        }

        public void UpdatePosition()
        {
            PositionChanged?.Invoke(this, new MockPositionChangedEventArgs(null));
        }
        
        public event EventHandler<MockPositionChangedEventArgs> PositionChanged;
    }
}