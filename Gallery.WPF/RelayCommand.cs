using System;
using System.Windows.Input;

namespace Gallery.WPF
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action commandTask;

        public RelayCommand(Action _commandTask)
        {
            commandTask = _commandTask;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            commandTask();
        }
    }
}
