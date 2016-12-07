using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MSCorp.FirstResponse.Client.ViewModels;

namespace MSCorp.FirstResponse.Client.Views
{
    public sealed partial class NewTicketPanePedestrianView : Page
    {
        public NewTicketPanePedestrianView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var incidentId = (int)e.Parameter;
            var viewModel = (NewTicketPaneMasterViewModel)this.DataContext;
            viewModel.Initialize(incidentId);
            base.OnNavigatedTo(e);
        }
    }
}
