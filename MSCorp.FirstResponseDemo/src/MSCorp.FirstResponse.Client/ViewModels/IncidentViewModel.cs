using System.Linq;
using MSCorp.FirstResponse.Client.Data;
using MSCorp.FirstResponse.Client.Models;

namespace MSCorp.FirstResponse.Client.ViewModels
{
    public class IncidentViewModel
    {
        public IncidentViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Incident = DataRepository.LoadIncidentData().First(x => x.Id == 10);
            }
        }

        public IncidentModel Incident { get; set; }

        public string RecievedTime => Incident.ReceivedTime.ToString("f");

        public void Initialize(int incidentId)
        {
            Incident = DataRepository.LoadIncidentData().First(x => x.Id == incidentId);
        }
    }
}
