using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MSCorp.FirstResponse.Client.ViewModels;

namespace MSCorp.FirstResponse.Client.Views
{
    public sealed partial class ResponderListPaneView : Page
    {
        public ResponderListPaneView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var viewModel = e.Parameter as ResponderListViewModel;
            if (viewModel != null)
            {
                DataContext = viewModel;
            }
            base.OnNavigatedTo(e);
        }
    }
}
