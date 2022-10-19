using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProvapharmNext.Commands
{
    public class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public CommandBase(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
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
