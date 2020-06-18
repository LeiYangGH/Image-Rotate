using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;

namespace ImageRotator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage originalImg;
        private string inputImageFileName;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "bmp|*.bmp";
            if (!dialog.ShowDialog() ?? false)
                return;
            this.inputImageFileName = dialog.FileName;
            //string fileName = @"C:\Users\LeiYang\Downloads\无标题.bmp";
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = new MemoryStream(File.ReadAllBytes(inputImageFileName));
            img.EndInit();
            this.originalImg = img;
            this.img.Source = img;
        }

        private void sldAngle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.originalImg == null)
                return;
            double angle = e.NewValue;
            this.img.RenderTransform = new RotateTransform(angle, this.originalImg.Width / 2, this.originalImg.Height / 2);
            //// Create the new BitmapSource that will be used to scale the size of the source.
            //TransformedBitmap myRotatedBitmapSource = new TransformedBitmap();

            //// BitmapSource objects like TransformedBitmap can only have their properties
            //// changed within a BeginInit/EndInit block.
            //myRotatedBitmapSource.BeginInit();

            //// Use the BitmapSource object defined above as the source for this BitmapSource.
            //// This creates a "chain" of BitmapSource objects which essentially inherit from each other.
            //myRotatedBitmapSource.Source = originalImg;

            //// Flip the source 90 degrees.
            //myRotatedBitmapSource.Transform = new RotateTransform(angle);
            //myRotatedBitmapSource.EndInit();
            //this.img.Source = myRotatedBitmapSource;
            //this.rotatedImg= new TransformedBitmap(, new RotateTransform(180));


        }


        public void SaveClipboardImageToFile(string filePath)
        {
            //var image = this.originalImg;
            var image = this.img.Source;
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                //encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)img.Source));
                encoder.Save(fileStream);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            double angle = (int)this.sldAngle.Value;
            string filename = Path.GetFileNameWithoutExtension(this.inputImageFileName);
            //SaveClipboardImageToFile(@"C:\Users\LeiYang\Downloads\rotate.bmp");
            string outputImageFileName = this.inputImageFileName.Replace(filename, filename + $"_旋转{angle}度");
            SaveToBmp(this.img, outputImageFileName);
            this.txtMsg.Text = $"已保存到{outputImageFileName}";
        }



        void SaveToBmp(FrameworkElement visual, string fileName)
        {
            var encoder = new BmpBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }



        void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }
    }
}
