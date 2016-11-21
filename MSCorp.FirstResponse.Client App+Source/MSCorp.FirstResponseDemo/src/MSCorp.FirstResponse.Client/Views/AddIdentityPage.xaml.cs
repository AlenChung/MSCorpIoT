using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MSCorp.FirstResponse.Client.Common;
using MSCorp.FirstResponse.Client.Data;
using MSCorp.FirstResponse.Client.Models;
using MSCorp.FirstResponse.Client.Services;

namespace MSCorp.FirstResponse.Client.Views
{
    /// <summary>
    /// The page that is used to add identities to an incident
    /// </summary>
    public sealed partial class AddIdentityPage : Page
    {
        private readonly List<SuspectModel> _suspects;
        private List<SearchResult> _femaleSearchResults = new List<SearchResult>();
        private List<SearchResult> _maleSearchResults = new List<SearchResult>();
        private List<SuspectModel> _suspectsToAdd;
        private int _incidentId;

        public AddIdentityPage()
        {
            InitializeComponent();
            _suspects = new List<SuspectModel>(DataRepository.LoadtSuspectData());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var incidentModel = (IncidentModel)e.Parameter;
            if (incidentModel != null) _incidentId = incidentModel.Id;

            var parameter = (IncidentModel)e.Parameter;
            if (parameter != null) _suspectsToAdd = parameter.Identities;

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// After the user has selected any persons they wish to add to the incident this method adds those
        /// persons to the incident and navigates to the incident details page
        /// </summary>
        private void OnDoneSelectingIdentitesClick(object sender, RoutedEventArgs e)
        {
            _suspectsToAdd.AddRange(_femaleSearchResults.Where(x => x.AddToIncident).Select(x => x.Suspect));
            _suspectsToAdd.AddRange(_maleSearchResults.Where(x => x.AddToIncident).Select(x => x.Suspect));

            SuspectService.AddSuspectsToIncident(_incidentId, _suspectsToAdd);

            Frame.Navigate(typeof(IncidentDetailsPaneView), _incidentId);
        }

        /// <summary>
        /// Applys the facets from the search and returns the suspects that match
        /// </summary>
        private void OnSearchButtonClick(object sender, RoutedEventArgs e)
        {
            // get facets if they have been set
            var selectedEyeColor = (string)(EyeColorSelectBox.SelectedItem as ComboBoxItem)?.Content;
            var selectedHairColor = (string)(HairColorSelectBox.SelectedValue as ComboBoxItem)?.Content;
            var selectedSex = (string)(SexSelectBox.SelectedValue as ComboBoxItem)?.Content;

            // filter results into a results list
            var filteredSuspects = new List<SuspectModel>(_suspects);

            if (selectedEyeColor != Constants.AnyFacet)
                filteredSuspects = filteredSuspects.Where(x => x.EyeColor == selectedEyeColor).ToList();

            if (selectedHairColor != Constants.AnyFacet)
                filteredSuspects = filteredSuspects.Where(x => x.HairColor == selectedHairColor).ToList();

            if (selectedSex != Constants.AnyFacet)
                filteredSuspects = filteredSuspects.Where(x => x.Sex == selectedSex).ToList();

            // set male and female result sets and counts, if there are no female results do not show the bottom border.
            _femaleSearchResults = filteredSuspects.Where(x => x.Sex == Constants.FemaleFacet).Select(x => new SearchResult { Suspect = x }).ToList();
            SearchResultsFemale.ItemsSource = _femaleSearchResults;
            var femaleCount = _femaleSearchResults.Count;
            FemaleResultCount.Text = femaleCount.ToString();
            BottomRectangle.Visibility = femaleCount > 0 ? Visibility.Visible : Visibility.Collapsed;

            _maleSearchResults = filteredSuspects.Where(x => x.Sex == Constants.MaleFacet).Select(x => new SearchResult { Suspect = x }).ToList();
            SearchResultsMale.ItemsSource = _maleSearchResults;
            MaleResultCount.Text = _maleSearchResults.Count.ToString();
        }

        /// <summary>
        /// Wrapper class for the suspect model and the bool that indicates to add the suspect to the incident
        /// </summary>
        internal class SearchResult
        {
            public SuspectModel Suspect { get; set; }
            public bool AddToIncident { get; set; }
        }
    }
}
