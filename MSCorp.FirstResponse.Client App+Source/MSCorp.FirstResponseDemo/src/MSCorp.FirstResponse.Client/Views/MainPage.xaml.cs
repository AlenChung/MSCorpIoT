using System;
using MSCorp.FirstResponse.Client.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MSCorp.FirstResponse.Client.Models;
using MSCorp.FirstResponse.Client.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MSCorp.FirstResponse.Client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DashboardViewModel _dashboardViewModel;
        private LoginViewModel _loginViewModel;
        public MainPage()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            this.InitializeComponent();
            ContentFrame.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (_dashboardViewModel == null)
            {
                _dashboardViewModel = new DashboardViewModel(Dispatcher, OnLogoutNavigation);
                ContentFrame.Navigate(typeof(DashboardPage), _dashboardViewModel);
            }

            if (_loginViewModel == null)
            {
                _loginViewModel = new LoginViewModel(OnAuthenticatedNavigation);
                LoginFrame.Navigate(typeof(LoginPage), _loginViewModel);
            }

            if (e.Parameter is IncidentSelectedEventArgs)
            {
                _dashboardViewModel.NavigateToDetailsPane(this, e.Parameter as IncidentSelectedEventArgs);
            }
        }

        private void OnAuthenticatedNavigation(UserRole authenticatedUser)
        {
            _dashboardViewModel.SelectedUser = authenticatedUser;
            LoginFrame.Visibility = Visibility.Collapsed;
            ContentFrame.Visibility = Visibility.Visible;
        }

        private void OnLogoutNavigation()
        {
            _loginViewModel.ClearCredentials();
            LoginFrame.Visibility = Visibility.Visible;
            ContentFrame.Visibility = Visibility.Collapsed;
        }
    }
}
