using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using UglyToad.PdfPig;

namespace models.Models
{
    public class PreparatFile : INotifyPropertyChanged
    {
        private bool _isSelected = false;

        public Preparat Parent { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsSelectedVisibility");
            }
        }

        public Visibility IsSelectedVisibility { get => IsSelected ? Visibility.Visible : Visibility.Hidden; }

        public FileInfo File { get; set; }

        public int CntPages { get; set; }


        public PreparatFile(string file)
        {
            File = new FileInfo(file);
            CntPages = LoadPageCount(file);
        }

        private static int LoadPageCount(string file)
        {
            try
            {
                using (var document = PdfDocument.Open(file))
                {
                    return document.NumberOfPages;
                }
            }
            catch
            {
                return 0;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
