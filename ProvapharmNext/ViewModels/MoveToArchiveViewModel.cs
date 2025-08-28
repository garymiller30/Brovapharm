using ProvapharmNext.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProvapharmNext.ViewModels
{
    public class MoveToArchiveViewModel
    {
        public ICommand ExportCommand { get; set; }
        public MoveToArchiveViewModel()
        {
            ExportCommand = new CommandBase(Execute, CanExecute);
        }
        private bool CanExecute(object parameter)
        {
            return true;
        }
        private void Execute(object parameter)
        {
            Debug.WriteLine(parameter);
        }
    }
}
