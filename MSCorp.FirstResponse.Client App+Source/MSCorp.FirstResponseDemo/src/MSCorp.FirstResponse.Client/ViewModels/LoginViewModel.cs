using MSCorp.FirstResponse.Client.Common;
using MSCorp.FirstResponse.Client.Data;
using MSCorp.FirstResponse.Client.Models;
using System;
using System.Linq;
using System.Windows.Input;

namespace MSCorp.FirstResponse.Client.ViewModels
{
    public class LoginViewModel: MainViewModelBase
    {
        private const string DefaultUser = "jclarkson";

        private readonly UserRole[] _userRoles;
        private Action<UserRole> _authenticatedFunction;


        public string UserName { get; set; }
        public string Password { get; set; }

        public LoginViewModel(Action<UserRole> authenticatedFunction)
        {
            _authenticatedFunction = authenticatedFunction;
            _userRoles = DataRepository.LoadUserRoles().ToArray();
            LoginCommand = LoginUserCommand();
        }

        public ICommand LoginCommand { get; }

        private DelegateCommand LoginUserCommand()
        {
            return new DelegateCommand(() => LoginUser());
        }
        private void LoginUser()
            {
            if (UserName.IsNotNullOrWhiteSpace() && Password.IsNotNullOrWhiteSpace())
            {
                var user = _userRoles.FirstOrDefault(x => string.Compare(x.UserName, UserName, StringComparison.OrdinalIgnoreCase) == 0);
                if (user == null)
                {
                    user = _userRoles.FirstOrDefault(x => string.Compare(x.UserName, DefaultUser, StringComparison.OrdinalIgnoreCase) == 0);
                }
                _authenticatedFunction?.Invoke(user);
            }
        }

        public void ClearCredentials()
        {
            UserName = string.Empty;
            Password = string.Empty;
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(Password));
        }
    }
}
