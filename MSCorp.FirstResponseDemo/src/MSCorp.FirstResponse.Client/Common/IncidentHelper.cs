using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;
using MSCorp.FirstResponse.Client.Models;

namespace MSCorp.FirstResponse.Client.Common
{
    public static class IncidentHelper
    {
        private static IEnumerable<IncidentTypeData> IncidentTypeDataList { get; } = new[]
        {
            new IncidentTypeData { IsPriority = false, IncidentType = IncidentType.Alert, Icon = new BitmapImage(new Uri("ms-appx:///Assets/icon/icon_alert.png")), Pin = new BitmapImage(new Uri("ms-appx:///Assets/pins/pin_alert.png"))},
            new IncidentTypeData { IsPriority = false, IncidentType = IncidentType.Animal, Icon = new BitmapImage(new Uri("ms-appx:///Assets/icon/icon_animal.png")), Pin = new BitmapImage(new Uri("ms-appx:///Assets/pins/pin_animal.png"))},
            new IncidentTypeData { IsPriority = false, IncidentType = IncidentType.Arrest, Icon = new BitmapImage(new Uri("ms-appx:///Assets/icon/icon_arrest.png")), Pin = new BitmapImage(new Uri("ms-appx:///Assets/pins/pin_arrest.png"))},
            new IncidentTypeData { IsPriority = false, IncidentType = IncidentType.Car, Icon = new BitmapImage(new Uri("ms-appx:///Assets/icon/icon_car.png")), Pin = new BitmapImage(new Uri("ms-appx:///Assets/pins/pin_car.png"))},
            new IncidentTypeData { IsPriority = false, IncidentType = IncidentType.Fire, Icon = new BitmapImage(new Uri("ms-appx:///Assets/icon/icon_fire.png")), Pin = new BitmapImage(new Uri("ms-appx:///Assets/pins/pin_siren.png"))},
            new IncidentTypeData { IsPriority = false, IncidentType = IncidentType.OfficerRequired, Icon = new BitmapImage(new Uri("ms-appx:///Assets/icon/icon_officer.png")), Pin = new BitmapImage(new Uri("ms-appx:///Assets/pins/pin_officer.png"))},
            new IncidentTypeData { IsPriority = false, IncidentType = IncidentType.Stranger, Icon = new BitmapImage(new Uri("ms-appx:///Assets/icon/icon_stranger.png")), Pin = new BitmapImage(new Uri("ms-appx:///Assets/pins/pin_stranger.png"))},
            new IncidentTypeData { IsPriority = true,  IncidentType = IncidentType.Car, Icon = new BitmapImage(new Uri("ms-appx:///Assets/icon/icon_car.png")), Pin = new BitmapImage(new Uri("ms-appx:///Assets/pins/pin_car.png"))},
        };

        public static IncidentTypeData GetIncidentData(IncidentType incidentType, bool? isPriority = null)
        {
            if (isPriority != null)
            {
                var result = IncidentTypeDataList.FirstOrDefault(x => x.IncidentType == incidentType && x.IsPriority == isPriority);
                if (result != null)
                {
                    return result;
                }
            }

            return IncidentTypeDataList.FirstOrDefault(x => x.IncidentType == incidentType);
        }
    }
}
