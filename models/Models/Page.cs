using System.Collections.ObjectModel;
using System.Diagnostics;

namespace models.Models
{
    public class Page
    {
        public int Id { get; set; }
        public double Width { get; set; } = 100;
        public double Height { get; set; } = 210;

        public string FilePath { get; set; }

        public ObservableCollection<Side> Sides { get; set; } = new ObservableCollection<Side>();

        public Page()
        {
            Sides.Add(new Side());
            Sides.Add(new Side());
        }
    }
}