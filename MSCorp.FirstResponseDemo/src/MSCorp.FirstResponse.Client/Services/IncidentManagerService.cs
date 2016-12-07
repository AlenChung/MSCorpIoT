using System;
using MSCorp.FirstResponse.Client.Controls;
using MSCorp.FirstResponse.Client.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using MSCorp.FirstResponse.Client.Common;

namespace MSCorp.FirstResponse.Client.Services
{
    public class IncidentManagerService
    {
        public event EventHandler<IncidentChangedEventArgs> IncidentAdded;
        public event EventHandler<IncidentChangedEventArgs> IncidentRemoved;

        private readonly Random _randGen = new Random(Configuration.RandomSeed);
        private readonly List<IncidentIcon> _incidentList;
        private readonly List<IncidentModel> _pendingIncidentModels;

        public IncidentManagerService()
        {
            _incidentList = new List<IncidentIcon>();
            _pendingIncidentModels = new List<IncidentModel>();
            Task.Factory.StartNew(UpdateIncidentLoop, TaskCreationOptions.LongRunning);
        }

        public async Task CreateIncidentIcon(IncidentModel incident)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    var icon = new IncidentIcon(incident);
                    _incidentList.Add(icon);
                    IncidentAdded?.Invoke(this, new IncidentChangedEventArgs(icon));
                });
        }

        private async void UpdateIncidentLoop()
        {
            while (true)
            {
                try
                {
                    var toRemoveList = _incidentList.Where(incidentIcon => incidentIcon.Incident.CurrentStatus == IncidentStatus.Resolved).ToList();
                    foreach (var resolvedIncident in toRemoveList)
                    {
                        await OnIncidentResolved(resolvedIncident);
                    }

                    await ManageIncidents();
                }
                catch (Exception ex)
                {
                    // Catch em all, Pokemon!
                    Debug.WriteLine("Exception in update incident loop: {0}", ex);
                }
                finally
                {
                    await Task.Delay(2000);
                }
            }
        }

        private async Task ManageIncidents()
        {
            if (_incidentList.Count < Configuration.MaxIncidentsInProgress)
            {
                var draw = _randGen.Next(100);
                if (draw < Configuration.RandomChanceToAddIncident)
                {
                    var newIncident = _pendingIncidentModels[_randGen.Next(_pendingIncidentModels.Count)];
                    await CreateIncidentIcon(newIncident);
                }
            }
        }

        private async Task OnIncidentResolved(IncidentIcon incidentIcon)
        {
            foreach (var responder in incidentIcon.Incident.Responders)
            {
                if (responder != null)
                {
                    var responderIcon = responder.ResponseUnit;
                    await responderIcon.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        responderIcon.UpdateStatus(ResponseStatus.Available);
                    });
                    responderIcon.Responder.Incident = null;
                    responderIcon.Responder.Request = null;
                }
            }

            IncidentRemoved?.Invoke(this, new IncidentChangedEventArgs(incidentIcon));
            RemoveIncidentFromActiveList(incidentIcon);
            var incident = incidentIcon.Incident;
            incident.ResetModel();
            AddIncidentToPendingList(incident);
        }

        public async void RemoveIncidentFromActiveList(IncidentIcon incident)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    _incidentList.Remove(incident);
                });
        }

        public async void AddIncidentToPendingList(IncidentModel incident)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    _pendingIncidentModels.Add(incident);
                });
        }

        public IncidentIcon GetIncidentIconById(int id)
        {
            return _incidentList.Find(x => x.Incident.Id == id);
        }
    }
}
