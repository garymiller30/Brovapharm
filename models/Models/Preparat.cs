using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace models.Models
{
    public class Preparat
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public ObservableCollection<PreparatFile> FileList { get; set; } = new ObservableCollection<PreparatFile>();

       
    }
}