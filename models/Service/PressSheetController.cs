using System.Collections.ObjectModel;
using models.Models;

namespace models.Service
{
    public class PressSheetController
    {
        public ObservableCollection<PressSheet> PressSheets { get; set; } = new ObservableCollection<PressSheet>();
    }
}