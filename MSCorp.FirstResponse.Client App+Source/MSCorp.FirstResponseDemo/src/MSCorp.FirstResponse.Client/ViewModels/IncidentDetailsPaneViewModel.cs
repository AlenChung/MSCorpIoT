using System.ComponentModel;
using System.Linq;
using MSCorp.FirstResponse.Client.Common;
using MSCorp.FirstResponse.Client.Data;
using MSCorp.FirstResponse.Client.Models;
using MSCorp.FirstResponse.Client.Services;

namespace MSCorp.FirstResponse.Client.ViewModels
{
    public class IncidentDetailsPaneViewModel : MainViewModelBase
    {

        private UserRole SelectedUser { get; set; }

        public string ReportingPartyText { get; set; }
        public string PhoneText { get; set; }

        public IncidentDetailsPaneViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Incident = DataRepository.LoadIncidentData().First();
            }
        }

        public IncidentModel Incident { get; set; }

        public string ReceivedTime => Incident.ReceivedTime.ToString("f");

        public void Initialize(int incidentId)
        {
            Incident = DataRepository.LoadIncidentData().First(x => x.Id == incidentId);
            SelectedUser = AuthenticationService.LoadAuthenticatedUser();
            AuthenticationService.RoleChanged += OnSelectedRoleChange;
            SetMaskedValues();
        }
        
        public void OnSelectedRoleChange(object sender, RoleChangedEventArgs e)
        {
            SelectedUser = e.SelectedRole;
            SetMaskedValues();
            OnPropertyChanged(nameof(ReportingPartyText));
            OnPropertyChanged(nameof(PhoneText));
        }

        private void SetMaskedValues()
        {
            if (SelectedUser.RoleName == Configuration.SuperUserRole)
            {
                // show un masked data
                ReportingPartyText = Incident.UnmaskedReportingParty;
                PhoneText = Incident.UnmaskedPhone;
            }
            else
            {
                ReportingPartyText = Incident.ReportingParty;
                PhoneText = Incident.Phone;
            }
        }
    }
}
