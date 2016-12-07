using MSCorp.FirstResponse.Client.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MSCorp.FirstResponse.Client.Views
{
    public sealed partial class IncidentPage : Page
    {
        private int _incidentId;

        public IncidentPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _incidentId = (int)e.Parameter;
            var viewModel = DataContext as IncidentViewModel;
            viewModel?.Initialize(_incidentId);
            OnIncidentToggleButtonClick(this, null);
            base.OnNavigatedTo(e);
        }

        private void OnIncidentToggleButtonClick(object sender, RoutedEventArgs e)
        {
            TicketToggleButton.IsChecked = false;
            TicketToggleButton.FontWeight = Windows.UI.Text.FontWeights.Normal;
            IncidentToggleButton.IsChecked = true;
            IncidentToggleButton.FontWeight = Windows.UI.Text.FontWeights.SemiBold;
            ListFrame.Navigate(typeof(IncidentDetailsPaneView), _incidentId);
        }

        private void OnTicketToggleButtonClick(object sender, RoutedEventArgs e)
        {
            TicketToggleButton.IsChecked = true;
            TicketToggleButton.FontWeight = Windows.UI.Text.FontWeights.SemiBold;
            IncidentToggleButton.IsChecked = false;
            IncidentToggleButton.FontWeight = Windows.UI.Text.FontWeights.Normal;
            ListFrame.Navigate(typeof(TicketsListPaneView), _incidentId);
        }
    }
}
