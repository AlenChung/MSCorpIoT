using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using MSCorp.FirstResponse.Client.Common;
using MSCorp.FirstResponse.Client.Controls;
using MSCorp.FirstResponse.Client.Data;
using MSCorp.FirstResponse.Client.Models;
using MSCorp.FirstResponse.Client.Services;
using MSCorp.FirstResponse.Client.Views;

namespace MSCorp.FirstResponse.Client.ViewModels
{
    public class DashboardViewModel : MainViewModelBase
    {
        private readonly IncidentManagerService _incidentIconManager;
        private readonly ResponderManagerService _responderManager;
        private readonly IncidentListViewModel _incidentListViewModel;
        private readonly ResponderListViewModel _responderListViewModel;
        private readonly Action _onLogoutNavigation;
        private MapControlService _mapService;
        private IncidentModel _selectedIncident;
        private IncidentIcon _selectedIncidentIcon;
        private UserRole _selectedUser;
        private bool _incidentToggleButtonChecked;
        private bool _responderToggleButtonChecked;
        private Visibility _toggleButtonGridVisibility;
        private bool _isMainViewZoom;
        private bool _isEnrouteToIncident;
        private Action<Type, object> _navigateFunction;

        public DashboardViewModel(CoreDispatcher dispatcher, Action onLogoutNavigation)
        {
            _onLogoutNavigation = onLogoutNavigation;
            if (DesignMode.DesignModeEnabled)
            {
                var incidentData = DataRepository.LoadIncidentData();
                foreach (var incident in incidentData)
                {
                    IncidentList.Add(incident);
                }

                SelectedIncident = IncidentList.First();
            }

            _isMainViewZoom = true;
            _isEnrouteToIncident = false;
            _incidentIconManager = new IncidentManagerService();
            _incidentIconManager.IncidentAdded += OnIncidentDataAdded;
            _incidentIconManager.IncidentRemoved += OnIncidentDataRemoved;
            _responderManager = new ResponderManagerService();
            _incidentListViewModel = new IncidentListViewModel();
            _responderListViewModel = new ResponderListViewModel(dispatcher);

            _responderManager.ResponderUpdated += OnResponderUpdated;
            _incidentListViewModel.IncidentSelected += OnIncidentIconSelected;

            LogoutButtonClickCommand = CommandWrapper(OnLogoutButtonClick);
            PowerBiClickCommand = CommandWrapper(OnPowerBiButtonClick);
            IncidentToggleButtonClickCommand = CommandWrapper(OnIncidentToggleButtonClick);
            ResponderToggleButtonClickCommand = CommandWrapper(OnResponderToggleButtonClick);
        }


        public DeviceResponderUnit DeviceResponderUnit { get; set; }
        public bool IsActionsEnabled { get; set; }
        public ObservableCollection<IncidentModel> IncidentList { get; } = new ObservableCollection<IncidentModel>();
        public ObservableCollection<UserRole> Users { get; } = new ObservableCollection<UserRole>();

        public Visibility SelectedIncidentDetailsVisible => SelectedIncident == null ? Visibility.Collapsed : Visibility.Visible;
        public Visibility PowerBiVisibility => SelectedUser != null && SelectedUser.RoleName == Configuration.SuperUserRole ? Visibility.Visible : Visibility.Collapsed;

        public ICommand PowerBiClickCommand { get; }
        public ICommand LogoutButtonClickCommand { get; }
        public ICommand IncidentToggleButtonClickCommand { get; }
        public ICommand ResponderToggleButtonClickCommand { get; }

        public Visibility ToggleButtonGridVisibility
        {
            get
            {
                return _toggleButtonGridVisibility;
            }
            set
            {
                _toggleButtonGridVisibility = value;
                OnPropertyChanged(nameof(ToggleButtonGridVisibility));
            }
        }

        public bool IncidentToggleButtonChecked
        {
            get
            {
                return _incidentToggleButtonChecked;
            }
            set
            {
                _incidentToggleButtonChecked = value;
                OnPropertyChanged(nameof(IncidentToggleButtonChecked));
                OnPropertyChanged(nameof(IncidentToggleButtonFontWeight));
            }
        }

        public bool ResponderToggleButtonChecked
        {
            get
            {
                return _responderToggleButtonChecked;
            }
            set
            {
                _responderToggleButtonChecked = value;
                OnPropertyChanged(nameof(ResponderToggleButtonChecked));
                OnPropertyChanged(nameof(ResponderToggleButtonFontWeight));
            }
        }
        
        public FontWeight IncidentToggleButtonFontWeight => _incidentToggleButtonChecked ? FontWeights.SemiBold : FontWeights.Normal;
        public FontWeight ResponderToggleButtonFontWeight => _responderToggleButtonChecked ? FontWeights.SemiBold : FontWeights.Normal;

        public UserRole SelectedUser
        {
            get { return _selectedUser; }
            set
            {   
                _selectedUser = value;
                _mapService?.OnNavigationBackRequested(this, null);
                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(PowerBiVisibility));
                AuthenticationService.AuthenticateUser(_selectedUser);
                RemoveSelectedIncident();
            }
        }

        public IncidentModel SelectedIncident
        {
            get { return _selectedIncident; }
            set
            {
                _selectedIncident = value;
                OnPropertyChanged(nameof(SelectedIncident));
                OnPropertyChanged(nameof(SelectedIncidentDetailsVisible));
            }
        }
        
        public void Initialize(MapControl mapControl, Action<Type, object> navigateFunction)
        {
            _navigateFunction = navigateFunction;
            var deviceResponderUnit = DataRepository.GetUser();
            var mapItems = new MapItemsControl();
            var selectedMapItems = new MapItemsControl();
            mapControl.Children.Add(mapItems);
            mapControl.Children.Add(selectedMapItems);
            _mapService = new MapControlService(mapControl, mapItems, selectedMapItems);
            _mapService.CreateResponderForPriorityIncidentHandler += _responderManager.CreateResponderForPriorityIncident;
            _mapService.IncidentDatailsHandler += OnIncidentDetailsNavigated;
            LoadResponders();
            _mapService.LoadResponder(deviceResponderUnit);
            LoadIncidents();
            LoadUserRoles();
            IncidentToggleButtonChecked = true;
            OnIncidentToggleButtonClick();
        }

        private void OnIncidentDetailsNavigated(object sender, IncidentSelectedEventArgs args)
        {
            if (args.ShowDetails == false)
            {
                _isMainViewZoom = true;
            }
            else
            {
                _isMainViewZoom = false;
                _isEnrouteToIncident = false;
            }
            ToggleDetailsPane(args.IncidentId, args.ShowDetails, true);
        }

        private void OnIncidentDataRemoved(object sender, IncidentChangedEventArgs args)
        {
            args.IncidentIcon.IncidentIconDetails -= OnIncidentIconSelected;
            _mapService.RemoveIncident(args.IncidentIcon);
            _incidentListViewModel.RemoveIncident(args.IncidentIcon.Incident);
            IncidentList.Remove(args.IncidentIcon.Incident);
        }

        private void OnIncidentDataAdded(object sender, IncidentChangedEventArgs args)
        {
            args.IncidentIcon.IncidentIconDetails += OnIncidentIconSelected;
            IncidentList.Add(args.IncidentIcon.Incident);
            _mapService.AddElements(args.IncidentIcon, args.IncidentIcon.Incident.GeoLocation, IncidentModel.AnchorPoint);
            _responderManager.AddAvailableIncidents(args.IncidentIcon);
            _incidentListViewModel.AddIncident(args.IncidentIcon.Incident);
        }
        
        private void LoadResponders()
        {
            var responderData = DataRepository.LoadResponderData();
            foreach (var responder in responderData)
            {
                var responderElement = _responderManager.AddResponder(responder);
                _mapService.AddElements(responderElement, responder.GeoLocation, ResponderModel.AnchorPoint);
            }
        }

        private async void LoadIncidents()
        {
            var incidentData = DataRepository.LoadIncidentData();
            foreach (var incidentItem in incidentData.Select((x, index) => new { Incident = x, Index = index }))
            {
                if (incidentItem.Index < Configuration.InitialIncidentStartAmount)
                {
                    await _incidentIconManager.CreateIncidentIcon(incidentItem.Incident);
                }
                else
                {
                    _incidentIconManager.AddIncidentToPendingList(incidentItem.Incident);
                }
            }
        }

        private void LoadUserRoles()
        {
            foreach (var userRole in DataRepository.LoadUserRoles())
            {
                Users.Add(userRole);
            }

            SelectedUser = Users.FirstOrDefault(x => x.RoleName == Configuration.AttendingOfficerRole);
        }

        private void OnResponderUpdated(object sender, EventArgs e)
        {
            _responderListViewModel.RefreshResponders();
        }

        private async void OnIncidentIconSelected(object sender, IncidentSelectedEventArgs e)
        {
            if (_isMainViewZoom)
            {
                SelectedIncident = e.ShowDetails ? IncidentList.FirstOrDefault(x => x.Id == e.IncidentId) : null;
                if (SelectedIncident != null)
                {
                    _selectedIncidentIcon = await _mapService.UpdateSelectedItem(SelectedIncident);
                    _selectedIncidentIcon.IncidentIconNavigate += OnEnrouteToIncident;

                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    SystemNavigationManager.GetForCurrentView().BackRequested += OnIncidentListBackRequested;
                }
                
                ToggleDetailsPane(e.IncidentId, e.ShowDetails, true);
                if (!_isEnrouteToIncident)
                {
                    NavigateToIncident();
                }
            }
        }

        private void OnIncidentListBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (_isMainViewZoom)
            {
                ToggleDetailsPane(-1, false, false);
                RemoveSelectedIncident();
            }
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnIncidentListBackRequested;
        }

        public void NavigateToIncident()
        {
            _mapService.ClearNavigation();
            if (SelectedIncident == null)
            {
                return;
            }
            _mapService.NavigateToIncident(SelectedIncident);
        }
        
        public async void RemoveSelectedIncident()
        {
            if (_selectedIncidentIcon != null)
            {
                _selectedIncidentIcon.IncidentIconNavigate -= OnEnrouteToIncident;
            }
            SelectedIncident = null;
            _selectedIncidentIcon = null;
            await _mapService.UpdateSelectedItem(null);
            if (!_isEnrouteToIncident)
            {
                _mapService.ClearNavigation();
            }
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnIncidentListBackRequested;
        }

        public void OnEnrouteToIncident(object sender, IncidentSelectedEventArgs e)
        {
            _mapService.MockTravelingToCurrentDestination(SelectedIncident);
            _isEnrouteToIncident = true;
        }

        public async void NavigateToDetailsPane(object sender, IncidentSelectedEventArgs args)
        {
            // always show tickets
            SelectedIncident = IncidentList.FirstOrDefault(x => x.Id == args.IncidentId);
            _selectedIncidentIcon = await _mapService.UpdateSelectedItem(SelectedIncident);
            ToggleDetailsPane(args.IncidentId, args.ShowDetails, true);
        }

        private void ToggleButtonVisibility(bool showButtons)
        {
            ToggleButtonGridVisibility = showButtons ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ToggleDetailsPane(int incidentId, bool showDetails, bool showTickets)
        {
            if (showDetails)
            {
                ToggleButtonVisibility(false);
                if (showTickets)
                {
                    _navigateFunction(typeof(IncidentPage), incidentId);
                }
                else
                {
                    _navigateFunction(typeof(IncidentDetailsPaneView), incidentId);
                }
            }
            else
            {
                ToggleButtonVisibility(true);
                if (IncidentToggleButtonChecked)
                {
                    _navigateFunction(typeof(IncidentListPaneView), _incidentListViewModel);
                }
                else
                {
                    _navigateFunction(typeof(ResponderListPaneView), _responderListViewModel);
                }
            }
        }

        public void OnMapControlMapTapped(MapControl sender, MapInputEventArgs args)
        {
            // if the currently selected incident is in "ReadyToIdentify" state then the officer is attending. 
            // disable click away while in attending view.
            if (SelectedIncident != null && !SelectedIncident.ReadyToIdentify)
            {
                ToggleDetailsPane(-1, false, false);
                RemoveSelectedIncident();
            }
        }
        
        public void OnUserRoleFlyoutOpened()
        {
            IsActionsEnabled = true;
            OnPropertyChanged(nameof(IsActionsEnabled));
        }

        public void OnUserRoleFlyoutClosed()
        {
            IsActionsEnabled = false;
            OnPropertyChanged(nameof(IsActionsEnabled));
        }

        private DelegateCommand CommandWrapper(Action action)
        {
            return new DelegateCommand(() =>
            {
                action?.Invoke();
            });
        }

        private void OnLogoutButtonClick()
        {
            _onLogoutNavigation?.Invoke();
        }

        private void OnPowerBiButtonClick()
        {
            var rootFrame = Window.Current.Content as Frame;
            rootFrame?.Navigate(typeof (PowerBIPage));
        }

        private void OnIncidentToggleButtonClick()
        {
            ResponderToggleButtonChecked = false;
            IncidentToggleButtonChecked = true;
            _navigateFunction(typeof(IncidentListPaneView), _incidentListViewModel);
        }

        private void OnResponderToggleButtonClick()
        {
            IncidentToggleButtonChecked = false;
            ResponderToggleButtonChecked = true;
            _navigateFunction(typeof(ResponderListPaneView), _responderListViewModel);
        }
        
    }
}
