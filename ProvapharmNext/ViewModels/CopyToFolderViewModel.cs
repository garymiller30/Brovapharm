using models.Models.json;
using Newtonsoft.Json;
using ProvapharmNext.Commands;
using ProvapharmNext.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProvapharmNext.ViewModels
{
    public  class CopyToFolderViewModel
    {
        public ICommand ExportCommand { get; set; }

        public CopyToFolderViewModel()
        {
            ExportCommand = new CommandBase(Execute, CanExecute);
        }
        private bool CanExecute(object parameter)
        {
            return true;
        }

        private void Execute(object parameter)
        {
            string exportPath = Settings.ExportPath;

            if (string.IsNullOrEmpty(exportPath))
            {
                var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
                if ((bool)dialog.ShowDialog())
                {
                    exportPath = dialog.SelectedPath;
                }
                else { return; }
            }

            List<FileItem> fileItems = new List<FileItem>();
            foreach (var preparat in Preparats.PreparatList)
            {
                var selectedFile = preparat.FileList.FirstOrDefault(x => x.IsSelected);

                FileItem fileItem = new FileItem();

                if (selectedFile == null)
                {
                    Notify.Error($"{preparat.Id}. {preparat.Name} без файлу");
                    return;
                }
                
                fileItem.path = selectedFile.File.FullName;
                fileItem.number = preparat.Id;
                fileItem.cntPages = selectedFile.CntPages;
                fileItem.preparatName = preparat.Name;
                fileItem.preparatNumber = preparat.Number;
                fileItem.quantity = preparat.Quantity;

                fileItems.Add(fileItem);
            }

            // копіюємо файли в папку, додаючи перед ім'ям номер препарату
            foreach (var item in fileItems)
            {
                string destFileName = Path.Combine(exportPath, $"{item.number}_{Path.GetFileNameWithoutExtension(item.path)}#{item.quantity}{Path.GetExtension(item.path)}");
                try
                {
                    File.Copy(item.path, destFileName, true);
                }
                catch (Exception e)
                {
                    Notify.Error(e.Message);
                    return;
                }

            }
            Notify.Information("Файли скопійовано!");

        }
    }
}
