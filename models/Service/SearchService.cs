using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using iTextSharp.text;
using models.Models;

namespace models.Service
{
    public class SearchService
    {
        public static void GetFilesForPreparats(GlobalSettings settings, ObservableCollection<Preparat> preparats)
        {
            foreach (var preparat in preparats)
            {
                
                var files = Directory.GetFiles(settings.ProductsRepository, $"*{preparat.Number}*.pdf");
                if (files.Any())
                {
                    foreach (string file in files)
                    {
                        preparat.FileList.Add(new PreparatFile(file){ Parent = preparat});
                    }

                    var sortedFiles = preparat.FileList.OrderByDescending(x=>x.File.CreationTime).FirstOrDefault();
                    if (sortedFiles != null) sortedFiles.IsSelected = true;

                }
            }
        }
    }
}