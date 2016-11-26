using System;
using Windows.Devices.Geolocation;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace MSCorp.FirstResponse.Client
{
    public static class Configuration
    {
        public const string MapKey = "IpjMhSrzSwXU1U21gEwr~Z71YXlz9Txfh28UIfKz49A~AuevJ7P1M0aOBmypIoOA8cWHygeIENrGXB9dc0yQkUgm2NLKUumG8-o1VOUGxPXD";
        public const double ZoomLevel = 16.1;    //If you want to Change your map zoom size
        public const double IncidentZoomLevel = 16;

<<<<<<< HEAD
        public static readonly Geopoint StartLocation = new Geopoint(new BasicGeoposition { Latitude = 25.045581, Longitude = 121.573011 });
=======
        public static readonly Geopoint StartLocation = new Geopoint(new BasicGeoposition { Latitude = 25.081182, Longitude = 121.586708 });
>>>>>>> 02c8822562fc23f5e0a97193f4833b92dc41aeea
        public static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(80);
        public static readonly BasicGeoposition[] PolygonSearch = {
            new BasicGeoposition { Latitude = 25.051556, Longitude = 121.554996 },//Denfender Net Check 
            new BasicGeoposition { Latitude = 25.050073, Longitude = 121.569591 },
            new BasicGeoposition { Latitude = 25.046407, Longitude = 121.571182 },
            new BasicGeoposition { Latitude = 25.043524, Longitude = 121.567135 },
            new BasicGeoposition { Latitude = 25.039857, Longitude = 121.567365 }
        };

        public static TimeSpan TimeToResolve { get; } = TimeSpan.FromSeconds(30);

        public const string SuperUserRole = "Supervisor";
        public const string AttendingOfficerRole = "Attending Officer";

        public const int RandomSeed = 23;
        public const int RandomChanceToAddIncident = 8;
        public const int InitialIncidentStartAmount = 12;
        
        public const double MovementSpeed = 0.0001; //0.00004;
        public const int MaxIncidentsInProgress = 15;

<<<<<<< HEAD
        public const double UserLatitude = 25.041687;    // This is my user point & user-car
        public const double UserLongitude = 121.562737;  
=======
        public const double UserLatitude = 25.041687;    //47.588400  This is my user point & user-car
        public const double UserLongitude = 121.562737;  //-122.035594
>>>>>>> 02c8822562fc23f5e0a97193f4833b92dc41aeea

        public const double MapStartLatitude = 25.038182;    //Allen UI Start Point with FirstResponse MS-TW
        public const double MapStartLongitude = 121.567362;

        public const double AmbulanceLatitude = 25.040373;
        public const double AmbulanceLongitude = 121.573194;

        public const string IncidentJsonDataFile = "Data/IncidentData.json";   //Aleardy Change with Taiwan Event
        public const string ResponderJsonDataFile = "Data/ResponderData.json";
        public const string TicketJsonDataFile = "Data/TicketData.json";
        public const string ResponderRoutesJsonDataFile = "Data/ResponderRoutes.json";
        public const string UserRolesJsonDataFile = "Data/UserRoles.json";
        public const string SuspectJsonDataFile = "Data/SuspectData.json";

        public static string PowerBiUrl   //PowerBi Url set-up
        {
            get
            {
                var values = ApplicationData.Current.LocalSettings.Values;
                var key = nameof(PowerBiUrl);
                if (values.ContainsKey(key))
                {
                    return values[key].ToString();
                }

                return "https://msit.powerbi.com/groups/me/dashboards/9e296530-b4fa-40ff-8968-cd425bdf490b";
            }
            set
            {
                var values = ApplicationData.Current.LocalSettings.Values;
                var key = nameof(PowerBiUrl);
                values[key] = value;
            }
        }
    }
}
