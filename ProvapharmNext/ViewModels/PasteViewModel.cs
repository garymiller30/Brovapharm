using models.Models;
using models.Service;
using ProvapharmNext.Commons;
using System;
using System.Linq;
using System.Threading.Tasks;
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

        private async void Execute(object parameter)
        {
            try
            {
                var _preparats = PasteService.GetPreparatsFromClipboard();
                await Task.Run(() => SearchService.GetFilesForPreparats(new GlobalSettings(), _preparats));
                _preparats.ToList().ForEach(Preparats.PreparatList.Add);
            }
            catch (Exception e)
            {
                Notify.Error(e.Message);
            }
        }
    }
}
