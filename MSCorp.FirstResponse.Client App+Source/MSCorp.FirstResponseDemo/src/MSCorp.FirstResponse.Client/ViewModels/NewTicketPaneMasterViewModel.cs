using System;
using System.Linq;
using MSCorp.FirstResponse.Client.Data;
using MSCorp.FirstResponse.Client.Services;

namespace MSCorp.FirstResponse.Client.ViewModels
{
    public class NewTicketPaneMasterViewModel
    {
        private static int _incidentId;
        public NewTicketPaneMasterViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Initialize(1);
            }
        }

        public string IssuingOfficer { get; } = AuthenticationService.LoadAuthenticatedUser().UserName;

        public string SuspectName
        {
            get
            {
                var suspects = SuspectService.GetSuspectsForIncident(_incidentId);
                return suspects.Count > 0 ? suspects.First().Name : string.Empty;
            }
        }

        public string Time { get; } = DateTime.Now.ToString("f");
        public string Location { get; set; }

        public void Initialize(int incidentId)
        {
            _incidentId = incidentId;
            var incident = DataRepository.LoadIncidentData().First(x => x.Id == incidentId);
            Location = incident.Address;
        }
    }
}
