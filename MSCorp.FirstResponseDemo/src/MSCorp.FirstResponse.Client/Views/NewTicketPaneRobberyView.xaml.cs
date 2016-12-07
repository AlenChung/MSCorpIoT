using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MSCorp.FirstResponse.Client.ViewModels;

namespace MSCorp.FirstResponse.Client.Views
{
    public sealed partial class NewTicketPaneRobberyView : Page
    {
        public NewTicketPaneRobberyView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var incidentId = (int)e.Parameter;
            var viewModel = (NewTicketPaneMasterViewModel)DataContext;
            viewModel.Initialize(incidentId);
            base.OnNavigatedTo(e);
        }
    }
}
