using System;
using System.Windows.Input;

namespace Gallery.WPF
{
    public class RelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<T> commandTask;

        public RelayCommand(Action<T> _commandTask)
        {
            commandTask = _commandTask;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            commandTask((T)parameter);
        }
    }

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
