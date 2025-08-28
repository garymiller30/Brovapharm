using models.Models;
using models.Models.json;
using models.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Notifications.Wpf;
using ProvapharmNext.Commons;
using ProvapharmNext.Controls;
using ProvapharmNext.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Path = System.IO.Path;

namespace ProvapharmNext
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private ObservableCollection<Preparat> _preparats;

        private PressSheetController _controller = new PressSheetController();
        private ImageSource defaultImage = new BitmapImage(new Uri("pack://application:,,,/Iconshock-Real-Vista-Medical-Emergency.ico"));
        private ObservableCollection<ImageFilePreview> _previewFiles;
        private GlobalSettings _settings;

        public MainWindow()
        {
            InitializeComponent();
            TreeViewPreparats.ItemsSource = Preparats.PreparatList;
            _settings = new GlobalSettings();
            LoadFromCommandLine();
        }

        private void LoadFromCommandLine()
        {
            var args = Environment.GetCommandLineArgs();

            // brovapharm.exe <word file>
            if (args.Count() == 2)
            {
                ProcessWordFile(args[1], _settings);
            }
            // brovapharm.exe <word file> <pdf files folder>
            else if (args.Count() == 3)
            {
                _settings.ProductsRepository = args[2];
                ProcessWordFile(args[1], _settings);
            }
            //else if (args.Count() == 4)
            //{
            //    _settings.LocalProductsRepository = args[2];
            //    // brovapharm.exe <word file> <pdf files folder> <repository>
            //    ProcessWordFile(args[1], _settings);
            //}
        }
        private void filesList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (TreeViewPreparats.SelectedItem is PreparatFile file && file.IsSelected == false)
                {
                    var res = MessageBox.Show("Move file to archive?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (res == MessageBoxResult.Yes)
                    {
                       if (MovePreparatToArchive(file))
                        {
                             // delete TreeViewPreparats.SelectedItem from TreeViewPreparats
                             var parent = file.Parent;
                            parent.FileList.Remove(file);
                             //var items = TreeViewPreparats.ItemsSource as IList;
                            //items?.Remove(file);
                        }

                    }
                }

                // Ваш код для видалення вибраного елемента
                //var selectedItem = filesList.SelectedItem;
                //if (selectedItem != null)
                //{
                //    // Наприклад, якщо ItemsSource - ObservableCollection<PreparatFile>
                //    var items = filesList.ItemsSource as IList;
                //    items?.Remove(selectedItem);
                //}
            }
        }

        private bool MovePreparatToArchive(PreparatFile file)
        {

            string targetDir = Path.Combine(_settings.ProductsRepository, "old");
            string targetFile = Path.Combine(targetDir, file.File.Name);
            if (File.Exists(targetFile))
            {
                var newFileName = $"{Path.GetFileNameWithoutExtension(file.File.Name)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(file.File.Name)}";
                targetFile = Path.Combine(targetDir, newFileName);

            }
            try
            {
                File.Move(file.File.FullName, targetFile);
            }
            catch (Exception)
            {

                return false;
            }

            return true;
        }

        IEnumerable<Preparat> ProcessWordFile(string wordFile, GlobalSettings settings)
        {
            IEnumerable<Preparat> preparats = null;

            var ext = Path.GetExtension(wordFile);
            if (ext.Equals(".docx", StringComparison.InvariantCultureIgnoreCase))
            {
                preparats = LoadOrderListFromWord.Load(wordFile);

                foreach (var preparat in preparats)
                {
                    Preparats.PreparatList.Add(preparat);
                }
                SearchService.GetFilesForPreparats(settings, Preparats.PreparatList);
                Settings.ExportPath = Path.GetDirectoryName(wordFile);
                var previewFiles = Directory.GetFiles(Settings.ExportPath, "*.jpg");
                _previewFiles = CreatePreviewFileList(previewFiles);
                filesList.ItemsSource = _previewFiles;
            }


            return preparats;
        }



        private void TreeViewPreparats_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is PreparatFile file)
            {
                TreeViewPreparats.ContextMenu = null;


                PdfViewerFront.GetPreviewPage(file.File.FullName, 1);
                PdfViewerBack.GetPreviewPage(file.File.FullName, 2);

                ImgBack.Source = file.Parent.BackPreview ?? defaultImage;
                ImgFront.Source = file.Parent.FrontPreview ?? defaultImage;
            }
            else
            {
                TreeViewPreparats.ContextMenu = TreeViewPreparats.Resources["EditMenu"] as ContextMenu;
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
                        file.Parent.FileList.Any(x => x.IsSelected = false);
                        file.IsSelected = true;
                        Clipboard.SetFileDropList(filePath);
                        ShowToolTip();
                    }
                    catch { }

                }

            }
        }

        private void ShowToolTip()
        {
            Notify.Information("Скопійовано у буфер обміну");
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

        private void filesList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ImageFilePreview file = (ImageFilePreview)filesList.SelectedItem;
            if (file != null)
            {
                // MessageBox.Show(file.Path);
            }
        }

        private void btnFront_Click(object sender, RoutedEventArgs e)
        {
            ImageFilePreview file = (ImageFilePreview)((System.Windows.Controls.Button)sender).DataContext;
            file.Color = new SolidColorBrush(Colors.LightBlue);
            ImgFront.Source = GetImageFromFile(file.Path);

            SetPreparatPreview();
            ImageScrollFront.ScrollToHorizontalOffset(ImgFront.ActualWidth / 2);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ImageFilePreview file = (ImageFilePreview)((System.Windows.Controls.Button)sender).DataContext;
            file.Color = new SolidColorBrush(Colors.LightBlue);
            ImgBack.Source = GetImageFromFile(file.Path);
            ImageScrollBack.ScrollToHorizontalOffset(ImgBack.ActualWidth / 2);
            SetPreparatPreview();
        }
    }
}
