using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MSCorp.FirstResponse.Client.ViewModels;

namespace MSCorp.FirstResponse.Client.Views
{
    public sealed partial class NewTicketPaneTrafficView : Page
    {
        public NewTicketPaneTrafficView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var incidentId = (int) e.Parameter;
            OnTrafficViolationTypeSelectionChanged(this, null);
            var viewModel = (NewTicketPaneMasterViewModel)this.DataContext;
            viewModel.Initialize(incidentId);
            base.OnNavigatedTo(e);
        }

        private void OnTrafficViolationTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DetailsSpeedingGrid.Visibility = SpeedingItem.IsSelected ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
