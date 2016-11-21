using System;
using Windows.UI.Xaml.Media.Imaging;

namespace MSCorp.FirstResponse.Client.Models
{
    public class DeviceResponderUnit : ResponderModel
    {
        public DeviceResponderUnit()
        {
            Latitude = Configuration.UserLatitude;
            Longitude = Configuration.UserLongitude; 
        }

        public BitmapImage ResponderPin => new BitmapImage(new Uri("ms-appx:///Assets/nav/nav_car.png"));
    }
}