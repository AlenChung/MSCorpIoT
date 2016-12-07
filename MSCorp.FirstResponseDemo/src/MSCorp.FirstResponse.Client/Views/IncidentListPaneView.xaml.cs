using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MSCorp.FirstResponse.Client.ViewModels;

namespace MSCorp.FirstResponse.Client.Views
{
    public sealed partial class IncidentListPaneView : Page
    {
        public IncidentListPaneView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var viewModel = e.Parameter as IncidentListViewModel;
            if (viewModel != null)
            {
                DataContext = viewModel;
            }
            base.OnNavigatedTo(e);
        }
    }
}
