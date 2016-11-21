using System;
using MSCorp.FirstResponse.Client.Data;
using MSCorp.FirstResponse.Client.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Core;
using MSCorp.FirstResponse.Client.Common;

namespace MSCorp.FirstResponse.Client.ViewModels
{
    public class IncidentListViewModel : MainViewModelBase
    {
        public event EventHandler<IncidentSelectedEventArgs> IncidentSelected;

        private IncidentModel _selectedIncident;

        public IncidentListViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                IncidentList = new ObservableCollection<IncidentModel>(DataRepository.LoadIncidentData());
            }
            else
            {
                IncidentList = new ObservableCollection<IncidentModel>();
            }
        }

        public ObservableCollection<IncidentModel> IncidentList { get; }

        public IncidentModel SelectedIncident
        {
            get { return _selectedIncident; }
            set
            {
                _selectedIncident = value;
                if (_selectedIncident != null)
                {
                    OnIncidentSelected(_selectedIncident.Id);
                }
                OnPropertyChanged(nameof(SelectedIncident));
            }
        }

        public async void AddIncident(IncidentModel incident)
        {
            if (IncidentList.All(x => x.Id != incident.Id))
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    IncidentList.Add(incident);
                });
            }
        }

        public async void RemoveIncident(IncidentModel incident)
        {
            if (IncidentList.Contains(incident))
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    IncidentList.Remove(incident);
                });
            }
        }

        protected virtual void OnIncidentSelected(int incidentId)
        {
            IncidentSelected?.Invoke(this, new IncidentSelectedEventArgs(incidentId));
        }
    }
}
