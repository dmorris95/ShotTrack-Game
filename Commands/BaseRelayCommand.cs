using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShotTrack.Commands
{
    public class BaseRelayCommand : ICommand
    { 
        //Base Relay Command for use with Binding functions to the Views to maintain MVVM
        private Predicate<object> _canExecute;
        private Action<object> _execute;

        public BaseRelayCommand(Predicate<object> canExecute, Action<object> execute)
        {
            this._canExecute = canExecute;
            this._execute = execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; } // Enables the command to be requeried
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
