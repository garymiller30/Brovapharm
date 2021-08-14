using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.UI.Xaml.Controls.Primitives;
using models.Models;
using models.Service;
using Notifications.Wpf;

namespace ProvapharmNext
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Preparat> _preparats;
        private NotificationManager _notificationManager = new NotificationManager();
        private PressSheetController _controller = new PressSheetController();
        

        public MainWindow()
        {
            InitializeComponent();


            

        }

        private void ButtonPaste_OnClick(object sender, RoutedEventArgs e)
        {
            _preparats = PasteService.GetPreparatsFromClipboard();
            SearchService.GetFilesForPreparats(new GlobalSettings(), _preparats);
            TreeViewPreparats.ItemsSource = _preparats;
            
            
        }


        private void TreeViewPreparats_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is PreparatFile file)
            {
                PdfViewerFront.GetPreviewPage(file.File.FullName,1);
                PdfViewerBack.GetPreviewPage(file.File.FullName,2);
            }
        }

        private void TreeViewPreparats_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeView tree)
            {
                if (tree.SelectedItem is PreparatFile file)
                {
                    var filePath = new StringCollection {file.File.FullName};
                    try
                    {
                        Clipboard.SetFileDropList(filePath);
                        ShowToolTip();
                    }
                    catch { }

                }

            }
        }

        private void ShowToolTip()
        {
            
            _notificationManager.Show(new NotificationContent
            {
                Title = "Бровафарм",
                Message = "Скопійовано у буфер обміну",
                Type = NotificationType.Information
            }, "WindowArea",TimeSpan.FromSeconds(2));
        }
    }
}
