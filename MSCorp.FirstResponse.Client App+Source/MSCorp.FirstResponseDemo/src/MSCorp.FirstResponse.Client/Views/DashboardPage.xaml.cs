using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using MSCorp.FirstResponse.Client.Common;
using MSCorp.FirstResponse.Client.ViewModels;

namespace MSCorp.FirstResponse.Client.Views
{
    public sealed partial class DashboardPage : Page
    {
        private DashboardViewModel _viewModel;

        public DashboardPage()
        {
            NavigationCacheMode = NavigationCacheMode.Required;
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            if (e.Parameter is DashboardViewModel)
            {
                _viewModel = e.Parameter as DashboardViewModel;
                DataContext = _viewModel;
                _viewModel.Initialize(MapControl, (pageType, parameter) => ListFrame.Navigate(pageType, parameter));
            }
        }

        private void OnMenuListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.RemovedItems.Count == 1)
            {
                UserRoleFlyout.Hide();
            }
        }

        private void OnMapControlMapTapped(MapControl sender, MapInputEventArgs args)
        {
            _viewModel.OnMapControlMapTapped(sender, args);
        }

        private void OnUserRoleFlyoutOpened(object sender, object e)
        {
            _viewModel.OnUserRoleFlyoutOpened();
        }

        private void OnUserRoleFlyoutClosed(object sender, object e)
        {
            _viewModel.OnUserRoleFlyoutClosed();
        }

        private void OnFlyoutItemClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UserRoleFlyout.Hide();
        }
    }
}
