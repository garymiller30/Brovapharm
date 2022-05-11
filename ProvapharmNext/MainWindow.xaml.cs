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
        private ImageSource defaultImage = new BitmapImage(new Uri("pack://application:,,,/Iconshock-Real-Vista-Medical-Emergency.ico"));
        private ObservableCollection<ImageFilePreview> _previewFiles;

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
                PdfViewerFront.GetPreviewPage(file.File.FullName, 1);
                PdfViewerBack.GetPreviewPage(file.File.FullName, 2);

                ImgBack.Source = file.Parent.BackPreview ?? defaultImage;
                ImgFront.Source = file.Parent.FrontPreview ?? defaultImage;
            }
        }

        private void TreeViewPreparats_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeView tree)
            {
                if (tree.SelectedItem is PreparatFile file)
                {
                    var filePath = new StringCollection { file.File.FullName };
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
            }, "WindowArea", TimeSpan.FromSeconds(2));
        }

        private void ImgFront_Drop(object sender, DragEventArgs e)
        {
            var images = GetImagesFromDrop(e);
            ImgFront.Source = images[0];
            ImgBack.Source = images.Length > 1 ? images[1] : ImgBack.Source;

            SetPreparatPreview();
            
            
        }

        private void SetPreparatPreview()
        {
            Preparat preparat = GetSelectedPreparat();
            if (preparat != null)
            {
                SetPreparatImages(preparat);

            }
        }

        private void SetPreparatImages(Preparat preparat)
        {
            preparat.FrontPreview = ImgFront.Source;
            preparat.BackPreview = ImgBack.Source;
        }

        private Preparat GetSelectedPreparat()
        {

            if (TreeViewPreparats.SelectedItem is PreparatFile file) return file.Parent;
            if (TreeViewPreparats.SelectedItem is Preparat preparat) return preparat;

            return null;


        }

        private void ImgBack_Drop(object sender, DragEventArgs e)
        {
            var images = GetImagesFromDrop(e);
            ImgBack.Source = images[0];
            ImgFront.Source = images.Length > 1 ? images[1] : ImgFront.Source;

            SetPreparatPreview();
        }

        private void ImgFront_DragOver(object sender, DragEventArgs e)
        {

            e.Handled = true;
        }

        private void ImgBack_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private BitmapImage GetImageFromDrop(DragEventArgs e)
        {
            BitmapImage bi = new BitmapImage();

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var file = files[0];
                bi.BeginInit();
                bi.UriSource = new Uri(file, UriKind.Absolute);
                bi.EndInit();
            }
            return bi;
        }

        private BitmapImage[] GetImagesFromDrop(DragEventArgs e)
        {
            var list = new List<BitmapImage>();

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.UriSource = new Uri(file, UriKind.Absolute);
                    bi.EndInit();
                    list.Add(bi);
                }
            }

            return list.ToArray();

        }

        private BitmapImage GetImageFromFile(string file)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(file, UriKind.Absolute);
            bi.EndInit();
            return bi;
        }

        private void buttonXchangeFrontBack_Click(object sender, RoutedEventArgs e)
        {
            if (TreeViewPreparats.SelectedItem is PreparatFile file)
            {
                ImgBack.Source = file.Parent.FrontPreview;
                ImgFront.Source = file.Parent.BackPreview;

                file.Parent.FrontPreview = ImgFront.Source;
                file.Parent.BackPreview = ImgBack.Source;

            }
        }

        private void filesList_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void filesList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                _previewFiles = CreatePreviewFileList(files);
                filesList.ItemsSource = _previewFiles;
            }
        }

        private ObservableCollection<ImageFilePreview> CreatePreviewFileList(string[] files)
        {
            var col = new ObservableCollection<ImageFilePreview>();
            foreach (var file in files)
            {
                col.Add(new ImageFilePreview(file));
            }
            return col;
        }

        private void filesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ImageFilePreview file = (ImageFilePreview)filesList.SelectedItem;
            if (file != null)
            {
               // MessageBox.Show(file.Path);
            }
        }

        private void btnFront_Click(object sender, RoutedEventArgs e)
        {
            ImageFilePreview file = (ImageFilePreview)((Button)sender).DataContext;
            ImgFront.Source = GetImageFromFile(file.Path);
            SetPreparatPreview();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ImageFilePreview file = (ImageFilePreview)((Button)sender).DataContext;
            ImgBack.Source = GetImageFromFile(file.Path);
            SetPreparatPreview();
        }
    }
}
