using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using iTextSharp.text.pdf;

namespace models.Models
{
    public class PreparatFile
    {
        public Preparat Parent {get;set;}

        public bool IsSelected { get; set; }
        public FileInfo File { get; set; }

        public int CntPages {get;set;}


        public PreparatFile(string file)
        {
            File = new FileInfo(file);

            using (var reader = new PdfReader(file))
            {
                CntPages = reader.NumberOfPages;
            }
        }
    }
}