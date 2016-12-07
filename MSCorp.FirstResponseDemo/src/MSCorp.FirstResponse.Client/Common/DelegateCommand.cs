using System;
using System.Windows.Input;

namespace MSCorp.FirstResponse.Client.Common
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public DelegateCommand(Action execute)
            : this(() => true, execute)
        {
        }

        public DelegateCommand(Func<bool> canExecute, Action execute)
        {
            if (canExecute == null)
            {
                throw new ArgumentNullException(nameof(canExecute));
            }

            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            _canExecute = canExecute;
            _execute = execute;
        }

        void ICommand.Execute(object arg)
        {
            if (_canExecute())
            {
                _execute();
            }
        }

        bool ICommand.CanExecute(object arg)
        {
            return _canExecute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
