using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Windows.Data.Pdf;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Streams;

namespace ProvapharmNext.Controls
{
    /// <summary>
    /// Interaction logic for PdfViewer.xaml
    /// </summary>
    public partial class PdfViewer : UserControl
    {
        

        public PdfViewer()
        {
            InitializeComponent();
            
        }

        public async void GetPreviewPage(string pathToFile, uint page)
        {
            if (!string.IsNullOrEmpty(pathToFile))
            {
                //making sure it's an absolute path

                var pdfFile = await StorageFile.GetFileFromPathAsync(pathToFile);
                var unwrap = await PdfDocument.LoadFromFileAsync(pdfFile);
                await PdfToImages(unwrap, page);
                //await StorageFile.GetFileFromPathAsync(pathToFile).AsTask()
                //    //load pdf document on background thread
                //    .ContinueWith(t => PdfDocument.LoadFromFileAsync(t.Result).AsTask()).Unwrap()
                //    //display on UI Thread
                //    .ContinueWith(t2 => PdfToImages( t2.Result,page), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
        private async Task PdfToImages( PdfDocument pdfDoc, uint pageNo)
        {
            var items = PagesContainer.Items;
            items.Clear();

            if (pdfDoc == null) return;

            if (pdfDoc.PageCount < pageNo) return;
            
            using (var page = pdfDoc.GetPage(pageNo-1))
            {
                var bitmap = await PageToBitmapAsync(page);
                var image = new Image
                {
                    Source = bitmap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 4, 0, 4),
                    MaxWidth = 800
                };
                items.Add(image);
            }
        }

        private  async Task<BitmapImage> PageToBitmapAsync(PdfPage page)
        {
            BitmapImage image = new BitmapImage();
            
            using (var stream = new InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(stream,new PdfPageRenderOptions()
                {
                    DestinationWidth = (uint)(page.Dimensions.MediaBox.Width * 2),
                    DestinationHeight = (uint)(page.Dimensions.MediaBox.Height * 2)
                });

                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream.AsStream();
                image.EndInit();
            }

            return image;
        }

    }
    
}
