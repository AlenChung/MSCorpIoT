using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MSCorp.FirstResponse.Client.Views
{
    public sealed partial class TicketsListPaneView : Page
    {
        private int _incidentId;

        public TicketsListPaneView()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _incidentId = (int)e.Parameter;
            OnIssueTicketClick(this, null);
            base.OnNavigatedTo(e);
        }

        private void OnIssueTicketClick(object sender, RoutedEventArgs e)
        {
            var parameters = new TicketPaneParameters {IncidentId = _incidentId, PaneFrame = Frame};
            TicketPane.Navigate(typeof (NewTicketPaneMasterView), parameters);
        }
    }

    public class TicketPaneParameters
    {
        public Frame PaneFrame { get; set; }
        public int IncidentId { get; set; }
    }
}
