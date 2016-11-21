using Windows.UI.Xaml;
using MSCorp.FirstResponse.Client.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MSCorp.FirstResponse.Client.Views
{
    public sealed partial class IncidentDetailsPaneView : Page
    {
        private int _incidentId;
        private IncidentDetailsPaneViewModel _incidentViewModel;

        public IncidentDetailsPaneView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _incidentId = (int)e.Parameter;
            _incidentViewModel = DataContext as IncidentDetailsPaneViewModel;
            _incidentViewModel?.Initialize(_incidentId);

            var showOffenders = _incidentViewModel != null && _incidentViewModel.Incident.ReadyToIdentify ? Visibility.Visible : Visibility.Collapsed;
            AddIdentityButton.Visibility = showOffenders;
            OffendersRectangle.Visibility = showOffenders;
            OffendersLabel.Visibility = showOffenders;

            base.OnNavigatedTo(e);
        }

        private void AddIdentityButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddIdentityPage), _incidentViewModel.Incident);
        }
    }
}
