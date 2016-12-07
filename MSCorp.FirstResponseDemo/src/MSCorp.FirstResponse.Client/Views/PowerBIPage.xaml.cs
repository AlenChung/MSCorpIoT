using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MSCorp.FirstResponse.Client.Views
{
    public sealed partial class PowerBIPage : Page
    {

        private bool _initialLoadComplete;
        public PowerBIPage()
        {
            InitializeComponent();
            PowerBiWebView.Source = new Uri(Configuration.PowerBiUrl);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Show back ui in title bar
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Visible;
            ProgressRing.IsActive = true;
        }

        private void PowerBiWebView_OnFrameNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (_initialLoadComplete) return;
            _initialLoadComplete = true;
            ProgressRing.IsActive = false;
        }
    }
}
