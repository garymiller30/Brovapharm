using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace models.Models
{
    public class ImageFilePreview : INotifyPropertyChanged
    {
        Brush color;
        public Brush Color { get => color; set { color = value; OnPropertyChanged(); } }
        public string Path { get; set; }
        public string Name { get; set; }

        public ImageFilePreview(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
