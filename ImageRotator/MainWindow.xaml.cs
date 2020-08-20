using Microsoft.Win32;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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

        private void OpenImageFile(string fileName)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = new MemoryStream(File.ReadAllBytes(fileName));
            img.EndInit();
            this.originalImg = img;
            this.img.Source = img;
            this.img.Width = this.originalImg.Width  ;
            this.img.Height =   this.originalImg.Height;
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "bmp|*.bmp";
            if (!dialog.ShowDialog() ?? false)
                return;
            this.inputImageFileName = dialog.FileName;
            OpenImageFile(this.inputImageFileName);
            //string fileName = @"C:\Users\LeiYang\Downloads\无标题.bmp";

        }


        private void SaveRotateImage()
        {
            double angle;
            if (double.TryParse(this.txtAngle.Text.Trim(), out angle))
            {
                string filename = Path.GetFileNameWithoutExtension(this.inputImageFileName);
                //SaveClipboardImageToFile(@"C:\Users\LeiYang\Downloads\rotate.bmp");
                string outputImageFileName = this.inputImageFileName.Replace(filename, filename + $"_旋转{angle}度");
                SaveToBmp(this.img, outputImageFileName, angle);
                this.txtMsg.Text = $"已保存到{outputImageFileName}";
            }
            else
            {
                this.txtMsg.Text = $"请输入合法的角度!";

            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveRotateImage();
        }



        void SaveToBmp(Image visual, string fileName, double angle)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder, angle);
        }



        void SaveUsingEncoder(Image imgControl, string fileName, BitmapEncoder encoder, double angle)
        {
            imgControl.UpdateLayout();
            //int hw = (int)Math.Sqrt(Math.Pow(this.originalImg.Width, 2) + Math.Pow(this.originalImg.Height, 2));
            //int hw = (int)Math.Sqrt(Math.Pow(this.originalImg.Width, 2) + Math.Pow(this.originalImg.Height, 2));
            //RenderTargetBitmap bitmap1 = new RenderTargetBitmap((int)imgControl.ActualWidth, (int)imgControl.ActualHeight, 96, 96, PixelFormats.Default);
            Size rotatedSize = GetRotatedSize(new Size(this.originalImg.Width, this.originalImg.Height), angle);
            //RenderTargetBitmap bitmap2 = new RenderTargetBitmap((int)imgControl.ActualWidth, (int)imgControl.ActualHeight, 96, 96, PixelFormats.Default);
            RenderTargetBitmap bitmap2 = new RenderTargetBitmap((int)rotatedSize.Width, (int)rotatedSize.Height, 96, 96, PixelFormats.Default);
            Rect bounds = VisualTreeHelper.GetDescendantBounds(imgControl);
            //using (Graphics bitmapBuffer = Graphics.FromImage(bitmap))
            //{
            //    System.Drawing.Brush brush = new SolidBrush(System.Drawing.Color.FromArgb(127, 255, 255, 255));
            //    bitmapBuffer.FillRectangle(brush, 10, 10, 80, 80);
            //}

            //DrawingVisual drawingVisual1 = new DrawingVisual();
            //using (DrawingContext drawingContext = drawingVisual1.RenderOpen())
            //{
            //    Brush visualBrush = Brushes.White;
            //    drawingContext.DrawRectangle(visualBrush, null,
            //      new Rect(new Point(), new Size((int)imgControl.ActualWidth, (int)imgControl.ActualHeight)));

            //}

            DrawingVisual drawingVisual2 = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual2.RenderOpen())
            {
                Brush visualBrush = new VisualBrush(imgControl);
                drawingContext.DrawRectangle(visualBrush, null,
                  //new Rect(new Point(), bounds.Size));
                  new Rect(new Point(), rotatedSize));

            }

            //bitmap.Render(drawingVisual);
            //bitmap1.Render(drawingVisual1);
            //bitmap2.Render(imgControl);
            bitmap2.Render(drawingVisual2);

            //BitmapFrame frame1 = BitmapFrame.Create(bitmap1);
            //encoder.Frames.Add(frame1);
            BitmapFrame frame2 = BitmapFrame.Create(bitmap2);
            encoder.Frames.Add(frame2);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        private void txtAngle_TextChanged(object sender, TextChangedEventArgs e)
        {
            double angle;
            if (double.TryParse(this.txtAngle.Text.Trim(), out angle))
            {
                this.img.RenderTransform = new RotateTransform(angle, this.originalImg.Width / 2, this.originalImg.Height / 2);
            }
        }

        private readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void txtAngle_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            //this.inputImageFileName = @"c:\tmp\input.bmp";
            this.inputImageFileName = @"c:\tmp\2.bmp";
            OpenImageFile(this.inputImageFileName);
            this.txtAngle.Text = "120";
            SaveRotateImage();
#endif
        }


        private Size GetRotatedSize(Size originalSize, double angle)
        {
            angle = Math.PI * angle / 180;
            int a = (int)(Math.Abs(originalSize.Width * Math.Sin(angle)) + Math.Abs(originalSize.Height * Math.Cos(angle)));
            int b = (int)(Math.Abs(originalSize.Width * Math.Cos(angle)) + Math.Abs(originalSize.Height * Math.Sin(angle)));
            return new Size(a, b);
        }
    }
}
