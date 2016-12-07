using System;
using MSCorp.FirstResponse.Client.Data;
using MSCorp.FirstResponse.Client.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Core;
using MSCorp.FirstResponse.Client.Common;

namespace MSCorp.FirstResponse.Client.ViewModels
{
    public class ResponderListViewModel : MainViewModelBase
    {
        private readonly CoreDispatcher _dispatcher;
        private DepartmentType _selectedDepartment;

        public ResponderListViewModel(CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            FullResponderList = DataRepository.LoadResponderData();
            ResponderList = new ObservableCollection<ResponderViewModel>();

            FilterResponders(DepartmentType.None);
            FilterClearRespondersCommand = GetFilterRespondersCommand(DepartmentType.None);
            FilterPoliceRespondersCommand = GetFilterRespondersCommand(DepartmentType.Police);
            FilterFireRespondersCommand = GetFilterRespondersCommand(DepartmentType.Fire);
            FilterMedicalRespondersCommand = GetFilterRespondersCommand(DepartmentType.Ambulance);
        }

        public ObservableCollection<ResponderViewModel> ResponderList { get; }
        public IList<ResponderModel> FullResponderList { get; }

        public ICommand FilterPoliceRespondersCommand { get; }
        public ICommand FilterFireRespondersCommand { get; }
        public ICommand FilterMedicalRespondersCommand { get; }
        public ICommand FilterClearRespondersCommand { get; }

        public bool AllRespondersChecked => _selectedDepartment == DepartmentType.None;
        public bool PoliceRespondersChecked => _selectedDepartment == DepartmentType.Police;
        public bool FireRespondersChecked => _selectedDepartment == DepartmentType.Fire;
        public bool MedicalRespondersChecked => _selectedDepartment == DepartmentType.Ambulance;
        
        public void FilterResponders(DepartmentType departmentFilterType)
        {
            ResponderList.Clear();
            var responders = FullResponderList;
            if (departmentFilterType != DepartmentType.None)
            {
                responders = responders.Where(x => x.ResponderDepartment == departmentFilterType).ToList();
            }

            foreach (var responder in responders)
            {
                ResponderList.Add(new ResponderViewModel(responder));
            }

            _selectedDepartment = departmentFilterType;
            OnPropertyChanged(nameof(AllRespondersChecked));
            OnPropertyChanged(nameof(PoliceRespondersChecked));
            OnPropertyChanged(nameof(FireRespondersChecked));
            OnPropertyChanged(nameof(MedicalRespondersChecked));
        }

        public async void RefreshResponders()
        {
            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (var responder in ResponderList)
                {
                    var model = FullResponderList.FirstOrDefault(x => x.Id == responder.Id);
                    if (model != null)
                    {
                        responder.UpdateStatus(model.Status.ToString(), model.StatusColor);
                    }
                }
            });
        }

        private DelegateCommand GetFilterRespondersCommand(DepartmentType departmentType)
        {
            return new DelegateCommand(() => FilterResponders(departmentType));
        }
    }
}
