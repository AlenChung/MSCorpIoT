using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using MSCorp.FirstResponse.Client.Common;

namespace MSCorp.FirstResponse.Client.Models
{
    public class IncidentModel
    {
        public readonly static Point AnchorPoint = new Point(0.5, 1);

        private IncidentType _incidentType;
        private bool _isHighPriority;
        
        public int Id { get; set; }

        public string CallNumber { get; set; }
        public string Phone { get; set; }
        public string UnmaskedPhone { get; set; }
        public string Title { get; set; }
        public DateTime ReceivedTime { get; set; }

        public string PropertyName { get; set; }
        public string Address { get; set; }

        public string ReportingParty { get; set; }
        public string UnmaskedReportingParty { get; set; }
        public string Description { get; set; }
        public string UpdateDescription { get; set; }

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public List<ResponderRequestModel> Responders { get; set; } = new List<ResponderRequestModel>();

        public bool ReadyToIdentify { get; set; }
        public List<SuspectModel> Identities { get; set; } = new List<SuspectModel>();
        public DateTime? FullyAttendedTime { get; set; }
        
        public Geopoint GeoLocation => new Geopoint(new BasicGeoposition { Latitude = Latitude, Longitude = Longitude });
        public Visibility IsHighPriorityVisiblity => (IsHighPriority) ? Visibility.Visible : Visibility.Collapsed;
        
        public IncidentType IncidentCategory
        {
            get { return _incidentType; }
            set
            {
                _incidentType = value;
                var data = IncidentHelper.GetIncidentData(_incidentType, IsHighPriority);
                IncidentIcon = data.Icon;
                IncidentPin = data.Pin;
            }
        }

        public BitmapImage IncidentIcon { get; set; }
        public BitmapImage IncidentPin { get; set; }
        
        public bool IsHighPriority
        {
            get
            {
                return _isHighPriority;
            }
            set
            {
                _isHighPriority = value;
                IncidentCategory = _incidentType;
            }
        }

        public int Priority => _isHighPriority ? 1 : 0;
        
        public IncidentStatus CurrentStatus
        {
            get
            {
                if (Responders.Any(x => !x.UnitResponded))
                {
                    return IncidentStatus.AwaitingResponders;
                }

                if (!FullyAttendedTime.HasValue)
                {
                    FullyAttendedTime = DateTime.Now;
                }
                
                if ((DateTime.Now - FullyAttendedTime) > Configuration.TimeToResolve)
                {
                    return IncidentStatus.Resolved;
                }

                return IncidentStatus.Resolving;
            }
        }

        public void ResetModel()
        {
            var newRequests = Responders.Select(responderRequest => new ResponderRequestModel(responderRequest.DepartmentType)).ToList();
            Responders = newRequests;
            FullyAttendedTime = null;
            ReceivedTime = DateTime.Now;
        }
    }
}
