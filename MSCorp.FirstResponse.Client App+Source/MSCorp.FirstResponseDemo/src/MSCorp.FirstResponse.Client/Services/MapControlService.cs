using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using MSCorp.FirstResponse.Client.Common;
using MSCorp.FirstResponse.Client.Controls;
using MSCorp.FirstResponse.Client.Data;
using MSCorp.FirstResponse.Client.Models;

namespace MSCorp.FirstResponse.Client.Services
{
    public class MapControlService
    {
        private readonly MapControl _mapControl;
        private readonly MapItemsControl _mapItems;
        private readonly MapItemsControl _selectedItems;
        private readonly MockGeolocator _geolocator;
        private ResponderIcon _userIcon;
        private MapRouteView _currentRoute;
        private IncidentModel _currentIncident;
        private MapPolygon _mapPolygon;
        private readonly IList<TicketIcon> _ticketIcons = new List<TicketIcon>();
        private bool _routeInAction;

        public event EventHandler<IncidentSelectedEventArgs> IncidentDatailsHandler;

        public delegate Task<ResponderRequestModel> CreateResponderForPriorityIncidentEventHandler(object sender, IncidentEventArgs e);
        public event CreateResponderForPriorityIncidentEventHandler CreateResponderForPriorityIncidentHandler;

        public MapControlService(MapControl mapControl, MapItemsControl mapItems, MapItemsControl selectedItems)
        {
            _mapControl = mapControl;
            _mapItems = mapItems;
            _selectedItems = selectedItems;
            _mapControl.MapServiceToken = Configuration.MapKey;
            MapService.ServiceToken = Configuration.MapKey;
            _geolocator = new MockGeolocator();
            
            var tickets = DataRepository.LoadTicketData();
            foreach (var ticket in tickets)
            {
                _ticketIcons.Add(new TicketIcon(ticket));
            }
        }

        public void LoadResponder(DeviceResponderUnit deviceResponderUnit)
        {
            _userIcon = new ResponderIcon(deviceResponderUnit);
            AddElements(_userIcon, deviceResponderUnit.GeoLocation, new Point(0.5, 0.5));
            CenterOnStartLocation();
        }

        public async void AddElements(DependencyObject incident, Geopoint location, Point anchorPoint)
        {
            await _mapControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _mapItems.Items.Add(incident);
                MapControl.SetLocation(incident, location);
                MapControl.SetNormalizedAnchorPoint(incident, anchorPoint);
            });
        }

        public async void RemoveElement(DependencyObject element)
        {
            await _mapControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _mapItems.Items.Remove(element);
            });
        }

        public async void NavigateToIncident(IncidentModel incident)
        {
            if (_routeInAction)
            {
                return;
            }
            var currentLoc = await GetUserLocation();
            // Get the route between the points.
            MapRoute routeResult = await ResolveMapRouteAsync(currentLoc, incident.GeoLocation);

            if (routeResult != null)
            {
                await _mapControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _currentRoute = new MapRouteView(routeResult);
                SolidColorBrush routeColor = (SolidColorBrush)Application.Current.Resources["RouteFillThemeBrush"];
                _currentRoute.RouteColor = routeColor.Color;
                _currentRoute.OutlineColor = Color.FromArgb(0, 0, 0, 0);

                    _mapControl.Routes.Add(_currentRoute);
                });
            }
        }

        public static async Task<MapRoute> ResolveMapRouteAsync(Geopoint fromLocation, Geopoint toLocation)
        {
            MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteAsync(
                fromLocation,
                toLocation,
                MapRouteOptimization.Time,
                MapRouteRestrictions.None);

            return routeResult.Status == MapRouteFinderStatus.Success ? routeResult.Route : null;
        }

        public async void ClearNavigation()
        {
            if (_userIcon.Responder.Status != ResponseStatus.EnRoute)
            {
                _currentRoute = null;
                _userIcon.IncidentResponsePath = null;
                _userIcon.RouteIndex = 0;
                _userIcon.RouteStepIndex = 0;
            }
            await _mapControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _mapControl.Routes.Clear();
            });
            _geolocator.UpdatePosition();
        }

        private async Task<Geopoint> GetUserLocation()
        {
            Geopoint currLoc = _userIcon.Responder.GeoLocation;
            var access = await Geolocator.RequestAccessAsync();

            if (access == GeolocationAccessStatus.Allowed)
            {
                currLoc = _geolocator.GetGeoposition();
            }
            return currLoc;
        }

        public async static void UpdateResponderPosition(ResponderIcon responderIcon, Geopoint geopoint)
        {
            await responderIcon.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MapControl.SetLocation(responderIcon, geopoint);
                responderIcon.Responder.GeoLocation = geopoint;
            });
        }

        private async void UpdateNavigationPosition()
        {
            if (_currentRoute != null && !_routeInAction)
            {
                _routeInAction = true;
                _userIcon.IncidentResponsePath = _currentRoute.Route.Path.Positions;
                var toastPositionIndex = _userIcon.IncidentResponsePath.Count / 2;

                bool complete;
                do
                {
                    complete = FollowCustomRoute(_userIcon);
                    if (toastPositionIndex == _userIcon.RouteIndex && _userIcon.RouteStepIndex == 0 && _currentIncident != null)
                    {
                        ToastHelper.PopIncidentUpdatedToast(_currentIncident.Id);
                        if (_currentIncident.UpdateDescription.IsNotNullOrWhiteSpace())
                        {
                            _currentIncident.Description = _currentIncident.UpdateDescription;
                        }
                    }

                    // stop user just before incident
                    var pathIndex = Math.Max(_userIcon.IncidentResponsePath.Count - 3, 0);
                    if (_userIcon.RouteIndex >= pathIndex)
                    {
                        complete = true;
                    }
                    await Task.Delay(Configuration.UpdateInterval);
                }
                while ((_currentRoute != null) && !complete);

                if (complete)
                {
                    _routeInAction = false;
                    _userIcon.UpdateStatus(ResponseStatus.Busy);
                    // navigate to the attend view
                    if (_currentIncident != null)
                    {
                        _currentIncident.ReadyToIdentify = true;
                        _mapControl.Center = _currentIncident.GeoLocation;
                        _mapControl.ZoomLevel = Configuration.IncidentZoomLevel;
                        await AddTickets();
                        await ShowPolygonSearch();
                        IncidentDatailsHandler?.Invoke(this, new IncidentSelectedEventArgs(_currentIncident.Id, true, false));
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                        SystemNavigationManager.GetForCurrentView().BackRequested += OnNavigationBackRequested;

                        // adding request for additional responder
                        if (_currentIncident.IsHighPriority && _currentIncident.Responders.All(x => x.DepartmentType != DepartmentType.Ambulance))
                        {
                            var invoke = CreateResponderForPriorityIncidentHandler?.Invoke(this, new IncidentEventArgs(_currentIncident));
                            if (invoke != null)
                            {
                                var responseRequestModel = await invoke;
                                AddElements(responseRequestModel.ResponseUnit, responseRequestModel.ResponseUnit.Responder.GeoLocation, ResponderModel.AnchorPoint);
                                _currentIncident.Responders.Add(responseRequestModel);
                            }
                        }
                    }
                    ClearNavigation();
                    await UpdateSelectedItem(null);
                }
            }

        }

        public void OnNavigationBackRequested(object sender, BackRequestedEventArgs e)
        {
            CenterOnStartLocation();
            ClearNavigation();
            ClearPolygon();
            IncidentDatailsHandler?.Invoke(this, new IncidentSelectedEventArgs(-1, false));
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnNavigationBackRequested;
        }

        private async void CenterOnStartLocation()
        {
            await _mapControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _mapControl.Center = new Geopoint(new BasicGeoposition { Longitude = Configuration.MapStartLongitude, Latitude = Configuration.MapStartLatitude });
                _mapControl.ZoomLevel = Configuration.ZoomLevel;
            });

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
                    double latSteps = Math.Abs(latDiff) / Configuration.MovementSpeed;
                    double lonSteps = Math.Abs(lonDiff) / Configuration.MovementSpeed;
                    responderIcon.RouteStepMax = (int)Math.Max(latSteps, lonSteps);
                    if (responderIcon.RouteStepMax > 0)
                    {
                        var latStepSpeed = Math.Abs(latDiff / responderIcon.RouteStepMax);
                        var lonStepSpeed = Math.Abs(lonDiff / responderIcon.RouteStepMax);
                        responderIcon.RouteStepLatitude = (latDiff > 0) ? latStepSpeed : latStepSpeed * -1;
                        responderIcon.RouteStepLongitude = (lonDiff > 0) ? lonStepSpeed : lonStepSpeed * -1;
                    }
                }
                else
                {
                    var lat = currPoint.Latitude + (responderIcon.RouteStepIndex * responderIcon.RouteStepLatitude);
                    var lon = currPoint.Longitude + (responderIcon.RouteStepIndex * responderIcon.RouteStepLongitude);
                    currPoint = new BasicGeoposition { Latitude = lat, Longitude = lon };
                }

                UpdateResponderPosition(responderIcon, new Geopoint(currPoint));
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

        public void CenterOnUserLocation(bool zoom = false)
        {
            _mapControl.Center = _userIcon.Responder.GeoLocation;
            if (zoom)
            {
                _mapControl.ZoomLevel = Configuration.ZoomLevel;
            }
        }

        public async Task AddTickets()
        {
            await _mapControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Show past tickets
                foreach (var ticket in _ticketIcons)
                {
                    if (!_mapItems.Items.Contains(ticket))
                    {
                        _mapItems.Items.Add(ticket);
                    }
                    MapControl.SetLocation(ticket, ticket.Model.Location);
                    MapControl.SetNormalizedAnchorPoint(ticket, new Point(0.5, 1));
                }
            });
        }

        public async Task ShowPolygonSearch()
        {
            await _mapControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // This can currently be called more than once if you switch navigation half way through to another event.
                if (_mapPolygon != null)
                {
                    _mapControl.MapElements.Remove(_mapPolygon);
                }

                // Show Polygon
                _mapPolygon = new MapPolygon
                {
                    Path = new Geopath(Configuration.PolygonSearch),
                    ZIndex = 1,
                    FillColor = Color.FromArgb(128, 128, 128, 128),
                    StrokeThickness = 0
                };
                _mapControl.MapElements.Add(_mapPolygon);
            });
        }

        public async void CenterOnIncidentLocation(IncidentModel incident)
        {
            await _mapControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Center and Zoom
                var center = incident.GeoLocation;
                _mapControl.Center = center;
                _mapControl.ZoomLevel = Configuration.IncidentZoomLevel;
            });
        }

        public void MockTravelingToCurrentDestination(IncidentModel selectedIncident)
        {
            _currentIncident = selectedIncident;
            UpdateNavigationPosition();
            _userIcon.UpdateStatus(ResponseStatus.EnRoute);
        }

        public async void RemoveIncident(IncidentIcon incident)
        {
            await _mapControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _mapItems.Items.Remove(incident);
            });
        }

        public async void ClearPolygon()
        {
            await _mapControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _mapControl.MapElements.Remove(_mapPolygon);
                _mapPolygon = null;
                foreach (var ticket in _ticketIcons)
                {
                    _mapItems.Items.Remove(ticket);
                }
            });
        }

        public async Task<IncidentIcon> UpdateSelectedItem(IncidentModel incident)
        {
            IncidentIcon icon = null;
            await _mapControl.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                _selectedItems.Items.Clear();
                if (incident != null)
                {
                    icon = new IncidentIcon(incident, true);
                    _selectedItems.Items.Add(icon);
                    MapControl.SetLocation(icon, incident.GeoLocation);
                    MapControl.SetNormalizedAnchorPoint(icon, new Point(0.5, 1));
                }
            });
            return icon;
        }
    }
}