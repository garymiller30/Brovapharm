using models.Models.json;
using models.Service;
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
    public class ExportViewModel
    {
        public ICommand ExportCommand { get;set;}

        public ExportViewModel()
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
                else { return;}
            }

            List<FileItem> fileItems = new List<FileItem>();
            foreach (var preparat in Preparats.PreparatList)
            {
                var selectedFile = preparat.FileList.FirstOrDefault(x => x.IsSelected);

                var fileItem = new FileItem();

                if (selectedFile == null) {
                    Notify.Error($"{preparat.Id}. {preparat.Name} без файлу");
                    return; }

                fileItem.path = selectedFile.File.FullName;
                fileItem.number = preparat.Id;
                fileItem.cntPages = selectedFile.CntPages;
                fileItem.preparatName = preparat.Name;
                fileItem.preparatNumber = preparat.Number;

                fileItems.Add(fileItem);
            }

            string jsonString = JsonConvert.SerializeObject(fileItems);

            try
            {
                File.WriteAllText(Path.Combine(exportPath, "files.json"), jsonString);
                Notify.Information("Список збережено!");
            }
            catch (Exception e)
            {

                Notify.Error(e.Message);
            }
            
            
        }
    }
}
