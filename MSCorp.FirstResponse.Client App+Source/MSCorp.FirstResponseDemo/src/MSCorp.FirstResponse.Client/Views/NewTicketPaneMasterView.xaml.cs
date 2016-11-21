using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MSCorp.FirstResponse.Client.Views
{
    public sealed partial class NewTicketPaneMasterView : Page
    {
        private Frame _containingFrame;
        private int _incidentId;

        public NewTicketPaneMasterView()
        {
            InitializeComponent();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                SpeedingItem.IsSelected = true;
                OnTicketTypeSelectionChanged(this, null);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameters = e.Parameter as TicketPaneParameters;
            _containingFrame = parameters.PaneFrame;
            _incidentId = parameters.IncidentId;
            base.OnNavigatedTo(e);
        }

        private void OnTicketTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpeedingItem.IsSelected)
            {
                NewTicketFrame.Navigate(typeof (NewTicketPaneTrafficView), _incidentId);
                FormActions.Visibility = Visibility.Visible;
            }
            else if (PedestrianItem.IsSelected)
            {
                NewTicketFrame.Navigate(typeof (NewTicketPanePedestrianView), _incidentId);
                FormActions.Visibility = Visibility.Visible;
            }
            else if (RobberyItem.IsSelected)
            {
                NewTicketFrame.Navigate(typeof(NewTicketPaneRobberyView), _incidentId);
                FormActions.Visibility = Visibility.Visible;
            }
            else if (ArrestItem.IsSelected)
            {
                NewTicketFrame.Navigate(typeof(NewTicketPaneArrestView), _incidentId);
                FormActions.Visibility = Visibility.Visible;
            }
            else if (OtherItem.IsSelected)
            {
                NewTicketFrame.Navigate(typeof(NewTicketPaneOtherView), _incidentId);
                FormActions.Visibility = Visibility.Visible;
            }
            else
            {
                FormActions.Visibility = Visibility.Collapsed;
            }
        }

        private void FormSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (_containingFrame.CanGoBack)
            {
                _containingFrame.GoBack();
            }
        }

        private void FormCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_containingFrame.CanGoBack)
            {
                _containingFrame.GoBack();
            }
        }
    }
}
