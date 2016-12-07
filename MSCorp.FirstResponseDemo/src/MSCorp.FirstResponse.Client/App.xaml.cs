using MSCorp.FirstResponse.Client.Views;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MSCorp.FirstResponse.Client.Common;

namespace MSCorp.FirstResponse.Client
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
//#if DEBUG
//            if (System.Diagnostics.Debugger.IsAttached)
//            {
//                this.DebugSettings.EnableFrameRateCounter = true;
//            }
//#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            // Register a global back event handler. This can be registered on a per-page-bases if you only have a subset of your pages
            // that needs to handle back or if you want to do page-specific logic before deciding to navigate back on those pages.
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

            var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            SolidColorBrush alternateColor = (SolidColorBrush)Current.Resources["AlternateBackgroundThemeBrush"];
            SolidColorBrush mainColor = (SolidColorBrush)Current.Resources["MainBackgroundThemeBrush"];
            titleBar.BackgroundColor = alternateColor.Color;
            titleBar.ButtonBackgroundColor = alternateColor.Color;
            titleBar.ButtonHoverBackgroundColor = mainColor.Color;
            titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonHoverForegroundColor = Windows.UI.Colors.White;
            titleBar.ForegroundColor = Windows.UI.Colors.White;
            titleBar.InactiveBackgroundColor = alternateColor.Color;
            titleBar.ButtonInactiveBackgroundColor = alternateColor.Color;
            titleBar.InactiveForegroundColor = Windows.UI.Colors.White;
            titleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.White;

            // Ensure the current window is active
            Window.Current.Activate();
            Window.Current.CoreWindow.KeyDown += ApplicationKeyDown();
        }

        private static TypedEventHandler<CoreWindow, KeyEventArgs> ApplicationKeyDown()
        {
            return async (s, e) =>
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                if (ctrl.HasFlag(CoreVirtualKeyStates.Down) && e.VirtualKey == VirtualKey.F4)
                {
                    var textBox = new TextBox {Text = Configuration.PowerBiUrl};
                    var dialog = new ContentDialog
                    {
                        Content = textBox,
                        Title = "PowerBI URL",
                        PrimaryButtonText = "Save",
                        SecondaryButtonText = "Cancel"
                    };

                    try
                    {
                        var result = await dialog.ShowAsync();
                        if (result == ContentDialogResult.Primary)
                        {
                            //Save power bi url
                            Configuration.PowerBiUrl = textBox.Text;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Only a single ContentDialog can be open at any time."))
                        {
                            //do nothing.
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            Window.Current.CoreWindow.KeyDown -= ApplicationKeyDown();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Invoked when a user issues a global back on the device.
        /// If the app has no in-app back stack left for the current view/frame the user may be navigated away
        /// back to the previous app in the system's app back stack or to the start screen.
        /// In windowed mode on desktop there is no system app back stack and the user will stay in the app even when the in-app back stack is depleted.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // If we can go back and the event has not already been handled, do so.
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        /// <summary>
        /// Invoked when the Activated event is raised, this can be raised from a toast notification being tapped
        /// </summary>
        /// <param name="e">The toast activated argments</param>
        protected override void OnActivated(IActivatedEventArgs e)
        {
            // if e is from a toast notification then cast it and strip the argument out.
            var argument = e as ToastNotificationActivatedEventArgs;
            if (argument == null)
            {
                return;
            }
            // strip the IncidentId out of the argument
            int incidentId = ToastHelper.GetIncidentNumber(argument.Argument);
            Window.Current.CoreWindow.KeyDown += ApplicationKeyDown();

            // Navigate to the incident
            Frame rootFrame = (Frame)Window.Current.Content;
            rootFrame.Navigate(typeof(MainPage), new IncidentSelectedEventArgs(incidentId));
        }


    }
}
