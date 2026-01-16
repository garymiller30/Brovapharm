using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace models.Models
{
    public class Preparat
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }

        public ImageSource FrontPreview {get;set;}
        public ImageSource BackPreview {get;set;}

        public ObservableCollection<PreparatFile> FileList { get; set; } = new ObservableCollection<PreparatFile>();

       
    }
}