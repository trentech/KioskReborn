using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KioskReborn
{
    public partial class WallpaperWindow : Window
    {
        public WallpaperWindow()
        {
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
