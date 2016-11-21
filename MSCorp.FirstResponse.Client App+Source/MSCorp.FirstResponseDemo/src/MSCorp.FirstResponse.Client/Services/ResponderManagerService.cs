using MSCorp.FirstResponse.Client.Models;
using MSCorp.FirstResponse.Client.Controls;
using System.Collections.Generic;
using MSCorp.FirstResponse.Client.Data;
using System.Threading.Tasks;
using System;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Core;
using MSCorp.FirstResponse.Client.Common;

namespace MSCorp.FirstResponse.Client.Services
{
    public class ResponderManagerService
    {
        private readonly IList<RouteModel> _routes;
        private readonly IList<IncidentModel> _availableIncidentList = new List<IncidentModel>();

        public ResponderManagerService()
        {
            _routes = DataRepository.LoadRoutes();
            Task.Factory.StartNew(UpdateResponderLoop, TaskCreationOptions.LongRunning);
        }

        public IList<ResponderIcon> ResponderList { get; } = new List<ResponderIcon>();
        public EventHandler ResponderUpdated;

        public async void AddAvailableIncidents(IncidentIcon incident)
        {
            await incident.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _availableIncidentList.Add(incident.Incident);
            });
        }

        public ResponderIcon AddResponder(ResponderModel responder)
        {
            var responderIcon = new ResponderIcon(responder);
            ResponderList.Add(responderIcon);
            return responderIcon;
        }

        private async void UpdateResponderLoop()
        {
            await ResolveRoutePaths();
            var updateInterval = Configuration.UpdateInterval;
            while (true)
            {
                try
                {
                    foreach (var responderIcon in ResponderList)
                    {
                        switch (responderIcon.Responder.Status)
                        {
                            case ResponseStatus.Available:
                                await FollowAvailibleRoute(responderIcon);
                                break;
                            case ResponseStatus.EnRoute:
                                await FollowIncidentRoute(responderIcon);
                                break;
                            case ResponseStatus.Busy:
                                await HandleResponderAvailiblity(responderIcon);
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    // Catch em all
                }
                finally
                {
                    await Task.Delay(updateInterval);
                }
            }
        }

        private async Task HandleResponderAvailiblity(ResponderIcon responderIcon)
        {
            if (responderIcon.Responder.Incident.CurrentStatus == IncidentStatus.Resolved)
            {
                await UpdateResponderStatus(responderIcon, ResponseStatus.Available);
                responderIcon.Responder.Incident = null;
            }
        }

        private async Task FollowAvailibleRoute(ResponderIcon responderIcon)
        {
            if (!responderIcon.OnPatrolRoute)
            {
                if (responderIcon.IncidentResponsePath == null)
                {
                    var route = _routes.FirstOrDefault(x => x.Id == responderIcon.Responder.RouteId);
                    var lastPoint = new Geopoint(route.RoutePoints.Last());
                    var routeMap =
                        await MapControlService.ResolveMapRouteAsync(responderIcon.Responder.GeoLocation, lastPoint);
                    responderIcon.IncidentResponsePath = routeMap.Path.Positions;
                    responderIcon.RouteIndex = 0;
                }

                var completedRoute = FollowCustomRoute(responderIcon);
                if (completedRoute)
                {
                    responderIcon.OnPatrolRoute = true;
                    responderIcon.IncidentResponsePath = null;
                    responderIcon.RouteIndex = 0;
                }
            }
            else
            {
                FollowDefaultRoute(responderIcon);
            }

            SearchForAvailibleIncident(responderIcon);
        }

        private async void SearchForAvailibleIncident(ResponderIcon responderIcon)
        {
            IncidentModel[] availibleForResponder = new IncidentModel[0];
            // Filter to responder type
            await responderIcon.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                availibleForResponder =
                    _availableIncidentList.Where(
                        x =>
                            x.Responders.Any(
                                y =>
                                    !y.UnitResponding && y.DepartmentType == responderIcon.Responder.ResponderDepartment))
                        .ToArray();
            });

            Tuple<MapRoute, IncidentModel> closestIncidentData = null;
            for (int i = 0; i < availibleForResponder.Length; i++)
            {
                var incident = availibleForResponder[i];
                if (incident != null)
                {
                    // Find Closest
                    var routeMap =
                        await
                            MapControlService.ResolveMapRouteAsync(responderIcon.Responder.GeoLocation,
                                incident.GeoLocation);
                    if (routeMap != null) // TODO: why would this be NULL?
                    {
                        if (closestIncidentData == null ||
                            routeMap.EstimatedDuration < closestIncidentData.Item1.EstimatedDuration)
                        {
                            closestIncidentData = new Tuple<MapRoute, IncidentModel>(routeMap, incident);
                        }
                    }
                }
            }

            // Assign if available incident found
            if (closestIncidentData != null)
            {
                var responderRequest =
                    closestIncidentData.Item2.Responders.FirstOrDefault(
                        x => !x.UnitResponding && x.DepartmentType == responderIcon.Responder.ResponderDepartment);
                if (responderRequest != null)
                {
                    responderRequest.RespondToRequest(responderIcon);
                    responderIcon.Responder.Request = responderRequest;
                    responderIcon.Responder.Incident = closestIncidentData.Item2;
                    responderIcon.RouteIndex = 0;
                    responderIcon.OnPatrolRoute = false;
                    responderIcon.IncidentResponsePath = closestIncidentData.Item1.Path.Positions;

                    await UpdateResponderStatus(responderIcon, ResponseStatus.EnRoute);
                }
            }

            // Clear any incidents no longer available
            await responderIcon.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var incidentsToRemove =
                    _availableIncidentList.Where(x => x.Responders.All(y => y.UnitResponding)).ToList();
                foreach (var incident in incidentsToRemove)
                {
                    _availableIncidentList.Remove(incident);
                }
            });
        }

        private async Task FollowIncidentRoute(ResponderIcon responderIcon)
        {
            var incident = responderIcon.Responder.Incident;
            if (incident != null)
            {
                if (responderIcon.IncidentResponsePath == null)
                {
                    var routeMap =
                        await
                            MapControlService.ResolveMapRouteAsync(responderIcon.Responder.GeoLocation,
                                incident.GeoLocation);
                    responderIcon.IncidentResponsePath = routeMap.Path.Positions;
                    responderIcon.RouteIndex = 0;
                    responderIcon.RouteStepIndex = 0;
                }

                var completedRoute = FollowCustomRoute(responderIcon);
                if (completedRoute)
                {
                    responderIcon.RouteIndex = 0;
                    responderIcon.RouteStepIndex = 0;
                    responderIcon.IncidentResponsePath = null;
                    responderIcon.IncidentArrivalTime = DateTime.Now;
                    responderIcon.Responder.Request.UnitResponded = true;
                    await UpdateResponderStatus(responderIcon, ResponseStatus.Busy);
                }
            }
            else
            {
                await UpdateResponderStatus(responderIcon, ResponseStatus.Available);
            }
        }

        private bool FollowCustomRoute(ResponderIcon responderIcon)
        {
            if (responderIcon.RouteIndex < (responderIcon.IncidentResponsePath.Count - 1))
            {
                var currPoint = responderIcon.IncidentResponsePath[responderIcon.RouteIndex];
                var nextPoint = responderIcon.IncidentResponsePath[responderIcon.RouteIndex + 1];

                if (responderIcon.RouteStepIndex == 0)
                {
                    var latDiff = (nextPoint.Latitude - currPoint.Latitude);
                    var lonDiff = (nextPoint.Longitude - currPoint.Longitude);
                    double latSteps = Math.Abs(latDiff)/Configuration.MovementSpeed;
                    double lonSteps = Math.Abs(lonDiff)/Configuration.MovementSpeed;
                    responderIcon.RouteStepMax = (int) Math.Max(latSteps, lonSteps);
                    if (responderIcon.RouteStepMax > 0)
                    {
                        var latStepSpeed = Math.Abs(latDiff/responderIcon.RouteStepMax);
                        var lonStepSpeed = Math.Abs(lonDiff/responderIcon.RouteStepMax);
                        responderIcon.RouteStepLatitude = (latDiff > 0) ? latStepSpeed : latStepSpeed*-1;
                        responderIcon.RouteStepLongitude = (lonDiff > 0) ? lonStepSpeed : lonStepSpeed*-1;
                    }
                }
                else
                {
                    var lat = currPoint.Latitude + (responderIcon.RouteStepIndex*responderIcon.RouteStepLatitude);
                    var lon = currPoint.Longitude + (responderIcon.RouteStepIndex*responderIcon.RouteStepLongitude);
                    currPoint = new BasicGeoposition {Latitude = lat, Longitude = lon};
                }

                MapControlService.UpdateResponderPosition(responderIcon, new Geopoint(currPoint));
                responderIcon.RouteStepIndex += 1;
                if (responderIcon.RouteStepIndex >= responderIcon.RouteStepMax)
                {
                    responderIcon.RouteIndex += 1;
                    responderIcon.RouteStepIndex = 0;
                }
                return false;
            }
            return true;
        }

        private void FollowDefaultRoute(ResponderIcon responderIcon)
        {
            if (responderIcon.IncidentResponsePath == null)
            {
                var route = _routes.FirstOrDefault(x => x.Id == responderIcon.Responder.RouteId);
                responderIcon.IncidentResponsePath = route.FullRoute.ToList();
                responderIcon.RouteIndex = 0;
                responderIcon.RouteStepIndex = 0;
            }

            var complete = FollowCustomRoute(responderIcon);
            if (complete)
            {
                responderIcon.RouteIndex = 0;
                responderIcon.RouteStepIndex = 0;
            }
        }

        private async Task UpdateResponderStatus(ResponderIcon responderIcon, ResponseStatus responseStatus)
        {
            await responderIcon.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                responderIcon.UpdateStatus(responseStatus);
            });
            ResponderUpdated?.Invoke(this, null);
        }

        private async Task ResolveRoutePaths()
        {
            foreach (var route in _routes)
            {
                var list = new List<BasicGeoposition>();
                var startPoint = route.RoutePoints.Last();
                foreach (var point in route.RoutePoints)
                {
                    var start = new Geopoint(startPoint);
                    var end = new Geopoint(point);
                    var routeMap = await MapControlService.ResolveMapRouteAsync(start, end);
                    startPoint = point;
                    list.AddRange(routeMap.Path.Positions);
                }
                route.FullRoute = list;
            }
        }

        public async Task<ResponderRequestModel> CreateResponderForPriorityIncident(object sender, IncidentEventArgs e)
        {
            var request = new ResponderRequestModel(DepartmentType.Ambulance);
            var incident = e.Incident;
            if (incident != null)
            {
                var responderModel = new ResponderModel
                {
                    Id = 8,
                    ResponderDepartment = DepartmentType.Ambulance,
                    Status = ResponseStatus.Available,
                    Longitude = Configuration.AmbulanceLongitude,
                    Latitude = Configuration.AmbulanceLatitude,
                    RouteId = 2
                };
                var responderIcon = new ResponderIcon(responderModel);
                request.RespondToRequest(responderIcon);

                responderIcon.Responder.Request = request;
                responderIcon.Responder.Incident = incident;
                responderIcon.RouteIndex = 0;
                responderIcon.OnPatrolRoute = false;
                await UpdateResponderStatus(responderIcon, ResponseStatus.EnRoute);
                ResponderList.Add(responderIcon);
            }
            return request;
        }
    }
}
