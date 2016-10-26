using System;
using System.Windows.Input;

namespace MuVaViMo.WPFDemo
{
    public class AlwaysExecutableRelayCommand : ICommand
    {
        private readonly Action<object> _command;
        public AlwaysExecutableRelayCommand(Action<object> command)
        {
            _command = command;
        }

        #region Implementation of ICommand

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _command.Invoke(parameter);

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}