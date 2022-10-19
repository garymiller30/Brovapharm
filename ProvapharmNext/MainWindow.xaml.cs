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

using models.Models.json;
using System.IO;
using Newtonsoft.Json;
using ProvapharmNext.ViewModels;
using ProvapharmNext.Controls;
using Path = System.IO.Path;
using ProvapharmNext.Commons;

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

        public MainWindow()
        {
            InitializeComponent();
            TreeViewPreparats.ItemsSource = Preparats.PreparatList;

            LoadFromCommandLine();
        }

        private void LoadFromCommandLine()
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Count() == 2)
            {
                var ext = Path.GetExtension(args[1]);
                // try to loading word file
                if (ext.Equals(".docx", StringComparison.InvariantCultureIgnoreCase)){
                    var preparats = LoadOrderListFromWord.Load(args[1]);

                    foreach (var preparat in preparats)
                    {
                        Preparats.PreparatList.Add(preparat);
                    }
                    SearchService.GetFilesForPreparats(new GlobalSettings(), Preparats.PreparatList);

                    Settings.ExportPath = Path.GetDirectoryName(args[1]);

                }
            }
        }

        //private void ButtonPaste_OnClick(object sender, RoutedEventArgs e)
        //{
        //    _preparats = PasteService.GetPreparatsFromClipboard();
        //    SearchService.GetFilesForPreparats(new GlobalSettings(), _preparats);
        //    TreeViewPreparats.ItemsSource = _preparats;
        //}

        //private void ButtonExport_OnClick(object sender, RoutedEventArgs e)
        //{

        //   var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
        //    if ((bool)dialog.ShowDialog()){
        //        ExportFileList(dialog.SelectedPath);
        //    }
        //}

        //private void ExportFileList(string path)
        //{
        //    List<FileItem> fileItems = new List<FileItem>();
        //    foreach (var preparat in _preparats)
        //    {
        //        var selectedFile = preparat.FileList.FirstOrDefault(x=>x.IsSelected);

        //        var fileItem = new FileItem();
        //        fileItem.path = selectedFile.File.FullName;
        //        fileItem.number = preparat.Id;
        //        fileItem.cntPages = selectedFile.CntPages;
        //        fileItem.preparatName = preparat.Name;
        //        fileItem.preparatNumber = preparat.Number;

        //        fileItems.Add(fileItem);
        //    }

        //    string jsonString = JsonConvert.SerializeObject(fileItems);
        //    File.WriteAllText(System.IO.Path.Combine(path,"files.json"), jsonString);

        //}

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
                        file.Parent.FileList.Any(x=>x.IsSelected = false);
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
            //var preparats = LoadOrderListFromWord.Load(@"F:\Jobs\USK\2022\#2022-10-17_USK_BROVAFARM_INSTRUKCII_111\Замовлення 111від 17.10.2022.docx");
            //SearchService.GetFilesForPreparats(new GlobalSettings(), new ObservableCollection<Preparat>( preparats));
            //  TreeViewPreparats.ItemsSource = preparats;
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
            file.Color =  new SolidColorBrush(Colors.LightBlue);
            ImgFront.Source = GetImageFromFile(file.Path);
            SetPreparatPreview();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            ImageFilePreview file = (ImageFilePreview)((Button)sender).DataContext;
            file.Color = new SolidColorBrush(Colors.LightBlue);
            ImgBack.Source = GetImageFromFile(file.Path);
            SetPreparatPreview();
        }
    }
}
