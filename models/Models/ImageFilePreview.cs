using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace models.Models
{
    public class ImageFilePreview
    {
        public string Path { get;set; }
        public string Name { get;set; }

        public ImageFilePreview(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path);
        }
    }
}
