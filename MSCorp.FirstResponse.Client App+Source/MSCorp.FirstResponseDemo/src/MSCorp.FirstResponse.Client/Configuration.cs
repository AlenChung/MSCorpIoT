using System;
using Windows.Devices.Geolocation;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace MSCorp.FirstResponse.Client
{
    public static class Configuration
    {
        public const string MapKey = "IpjMhSrzSwXU1U21gEwr~Z71YXlz9Txfh28UIfKz49A~AuevJ7P1M0aOBmypIoOA8cWHygeIENrGXB9dc0yQkUgm2NLKUumG8-o1VOUGxPXD";
        public const double ZoomLevel = 14.1;
        public const double IncidentZoomLevel = 16;

        public static readonly Geopoint StartLocation = new Geopoint(new BasicGeoposition { Latitude = 47.597893, Longitude = -122.01724 });
        public static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(80);
        public static readonly BasicGeoposition[] PolygonSearch = {
            new BasicGeoposition { Latitude = 47.584540, Longitude = -122.022430 },
            new BasicGeoposition { Latitude = 47.580847, Longitude = -122.022217 },
            new BasicGeoposition { Latitude = 47.577210, Longitude = -122.014425 },
            new BasicGeoposition { Latitude = 47.577954, Longitude = -122.003700 },
            new BasicGeoposition { Latitude = 47.584410, Longitude = -122.003914 }
        };

        public static TimeSpan TimeToResolve { get; } = TimeSpan.FromSeconds(30);

        public const string SuperUserRole = "Supervisor";
        public const string AttendingOfficerRole = "Attending Officer";

        public const int RandomSeed = 23;
        public const int RandomChanceToAddIncident = 8;
        public const int InitialIncidentStartAmount = 12;
        
        public const double MovementSpeed = 0.0001; //0.00004;
        public const int MaxIncidentsInProgress = 15;

        public const double UserLatitude = 47.588400;
        public const double UserLongitude = -122.035594;

        public const double MapStartLatitude = 47.601631;
        public const double MapStartLongitude = -122.062454;

        public const double AmbulanceLatitude = 47.575516;
        public const double AmbulanceLongitude = -121.988506;

        public const string IncidentJsonDataFile = "Data/IncidentData.json";
        public const string ResponderJsonDataFile = "Data/ResponderData.json";
        public const string TicketJsonDataFile = "Data/TicketData.json";
        public const string ResponderRoutesJsonDataFile = "Data/ResponderRoutes.json";
        public const string UserRolesJsonDataFile = "Data/UserRoles.json";
        public const string SuspectJsonDataFile = "Data/SuspectData.json";

        public static string PowerBiUrl
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
