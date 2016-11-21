using Windows.UI.Xaml.Media;
using MSCorp.FirstResponse.Client.Models;

namespace MSCorp.FirstResponse.Client.ViewModels
{
    public class ResponderViewModel: MainViewModelBase
    {
        public ResponderViewModel(ResponderModel model)
        {
            Id = model.Id;
            ResponderDepartment = model.ResponderDepartment.ToString();
            ResponderCode = model.ResponderCode;
            Status = model.Status.ToString();
            StatusColor = model.StatusColor;
        }

        public int Id { get; set; }
        public string ResponderDepartment { get; set; }
        public string Status { get; set; }
        public string ResponderCode { get; set; }
        public Brush StatusColor { get; set; }

        public void UpdateStatus(string status, Brush statusColor)
        {
            Status = status;
            StatusColor = statusColor;
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(StatusColor));
        }
    }
}
