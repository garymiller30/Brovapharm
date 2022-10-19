using models.Models;
using models.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProvapharmNext.ViewModels
{
    public class PasteViewModel
    {
        public ICommand PasteCommand { get;set;}

        public PasteViewModel()
        {
            PasteCommand = new Commands.CommandBase(Execute,CanExecute);
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        private void Execute(object parameter)
        {
            var _preparats = PasteService.GetPreparatsFromClipboard();
            SearchService.GetFilesForPreparats(new GlobalSettings(), _preparats);
            _preparats.ToList().ForEach(Preparats.PreparatList.Add);
            

           // TreeViewPreparats.ItemsSource = _preparats;
        }
    }
}
