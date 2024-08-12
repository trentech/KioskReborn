using KioskRebornLib;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KioskReborn
{
    public partial class WallpaperWindow : Window
    {
        public WallpaperWindow()
        {
            string images = Path.Combine(Settings.PATH, "Images");

            if (!Directory.Exists(images))
            {
                Directory.CreateDirectory(images);

                Bitmap image = Properties.Resources.calculator;
                image.Save(Path.Combine(images, "calculator.png"));

                image = Properties.Resources.notepad;
                image.Save(Path.Combine(images, "notepad.png"));

                image = Properties.Resources.background;
                image.Save(Path.Combine(images, "background.jpg"));

                image = Properties.Resources.lju_background;
                image.Save(Path.Combine(images, "lju_background.jpg"));

                image = Properties.Resources.WPS_Retriever.ToBitmap();
                image.Save(Path.Combine(images, "WPS_Retriever.ico"));

                image = Properties.Resources.LJU_PlotFetcher.ToBitmap();
                image.Save(Path.Combine(images, "LJU_PlotFetcher.ico"));
            }

            InitializeComponent();

            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;

            Settings settings = Settings.Get();

            Application.Current.Resources.MergedDictionaries.Clear();

            switch (settings.Color)
            {
                case Settings.Colors.Gray:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/Gray.xaml", UriKind.Relative) });
                    break;
                case Settings.Colors.Blue:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/Blue.xaml", UriKind.Relative) });
                    break;
                case Settings.Colors.Classic:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/Classic.xaml", UriKind.Relative) });
                    break;
                case Settings.Colors.Green:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/Green.xaml", UriKind.Relative) });
                    break;
                case Settings.Colors.Dark:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/Dark.xaml", UriKind.Relative) });
                    break;
                case Settings.Colors.LJU:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Themes/LJU.xaml", UriKind.Relative) });
                    break;
                default:
                    break;
            }

            ImageBrush brush = new ImageBrush();
            brush.ImageSource = ConvertImage(System.Drawing.Image.FromFile(settings.Background));
            Background = brush;

            TaskBarWindow taskbar = new TaskBarWindow();
            taskbar.Show();
        }

        public static ImageSource ConvertImage(System.Drawing.Image image)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();

            return bitmapImage;
        }
    }
}
