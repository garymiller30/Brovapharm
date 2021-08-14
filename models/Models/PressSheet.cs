using System.Collections.ObjectModel;
using System.Linq;

namespace models.Models
{
    public class PressSheet
    {
        private PressSheetType Type { get; set; } = PressSheetType.TwoSide;

        public double Width { get; set; } = 450;
        public double Height { get; set; } = 320;

        public ObservableCollection<Page> PageList { get; set; } = new ObservableCollection<Page>();

        public PressSheet()
        {
            PageList.Add(new Page());
            PageList.Add(new Page());
            PageList.Add(new Page());
            PageList.Add(new Page());
            PageList.Add(new Page());
            PageList.Add(new Page());
        }
    }
}