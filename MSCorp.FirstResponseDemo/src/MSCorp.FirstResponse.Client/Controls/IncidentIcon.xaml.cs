using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MSCorp.FirstResponse.Client.Models;
using Windows.UI.Xaml;
using MSCorp.FirstResponse.Client.Common;

namespace MSCorp.FirstResponse.Client.Controls
{
    public sealed partial class IncidentIcon : UserControl
    {
        public event EventHandler<IncidentSelectedEventArgs> IncidentIconDetails;
        public event EventHandler<IncidentSelectedEventArgs> IncidentIconNavigate;

        public IncidentIcon(IncidentModel incident, bool menuVisible = false)
        {
            InitializeComponent();

            Incident = incident;
            if (Incident.IsHighPriority)
            {
                PriorityIconStoryboard.Begin();
            }

            var visibility = Incident.IsHighPriority ? Visibility.Visible : Visibility.Collapsed;
            PriorityImage.Visibility = BackEllipse.Visibility = PulseEllipse.Visibility = visibility;
            IconImage.Source = Incident.IncidentPin;
            ButtonOne.Visibility = menuVisible ? Visibility.Visible : Visibility.Collapsed;

            IncidentTitle.Text = Incident.Title;
            IncidentDetails.Text = Incident.Description;
            IncidentLocation.Text = Incident.Address;
        }

        public IncidentModel Incident { get; }
        private void OnIncidentIconDetails(int incidentId, bool detailsVisible) => IncidentIconDetails?.Invoke(this, new IncidentSelectedEventArgs(incidentId, detailsVisible));
        private void OnIncidentIconNavigate(int incidentId) => IncidentIconNavigate?.Invoke(this, new IncidentSelectedEventArgs(incidentId));

        private void OnIconImageTapped(object sender, TappedRoutedEventArgs e)
        {
            var currentVis = ButtonOne.Visibility;
            OnIncidentIconDetails(Incident.Id, currentVis == Visibility.Collapsed);
        }

        private void OnDetailsButtonClick(object sender, RoutedEventArgs e)
        {
            OnIncidentIconDetails(Incident.Id, true);
        }

        private void OnNavigateButtonClick(object sender, RoutedEventArgs e)
        {
            OnIncidentIconNavigate(Incident.Id);
        }
    }
}
