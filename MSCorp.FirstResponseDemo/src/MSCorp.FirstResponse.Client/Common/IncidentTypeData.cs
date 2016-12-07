using Windows.UI.Xaml.Media.Imaging;
using MSCorp.FirstResponse.Client.Models;

namespace MSCorp.FirstResponse.Client.Common
{
    public class IncidentTypeData
    {
        public BitmapImage Icon { get; set; }
        public BitmapImage Pin { get; set; }
        public IncidentType IncidentType { get; set; }
        public bool IsPriority { get; set; }
    }
}