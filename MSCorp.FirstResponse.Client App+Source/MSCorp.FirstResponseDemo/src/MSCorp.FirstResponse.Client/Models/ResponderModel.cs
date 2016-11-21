using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace MSCorp.FirstResponse.Client.Models
{
    public class ResponderModel
    {
        public static readonly Point AnchorPoint = new Point(0.5, 0.5);

        public int Id { get; set; }
        public DepartmentType ResponderDepartment { get; set; }
        public ResponseStatus Status { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int IncidentId { get; set; }

        //Used to configure responder default route
        public int RouteId { get; set; }
        public IncidentModel Incident { get; set; }
        public ResponderRequestModel Request { get; set; }
        
        public Geopoint GeoLocation
        {
            get { return new Geopoint(new BasicGeoposition { Latitude = Latitude, Longitude = Longitude }); }
            set { Latitude = value.Position.Latitude; Longitude = value.Position.Longitude; }
        }

        public string ResponderInitial
        {
            get
            {
                switch (ResponderDepartment)
                {
                    case DepartmentType.Fire:
                        return "F";
                    case DepartmentType.Police:
                        return "P";
                    case DepartmentType.Ambulance:
                        return "M";
                    default:
                        return "P";
                }
            }
        }

        public string ResponderCode => string.Format("{0}{1}", ResponderInitial, Id);


        public SolidColorBrush StatusColor
        {
            get
            {
                switch (Status)
                {
                    case ResponseStatus.Available: return new SolidColorBrush(Colors.Green);
                    case ResponseStatus.EnRoute: return new SolidColorBrush(Colors.Blue);
                    case ResponseStatus.Busy: return new SolidColorBrush(Colors.Red);
                    default: return new SolidColorBrush(Colors.Green);
                }
            }
        }
    }
}
