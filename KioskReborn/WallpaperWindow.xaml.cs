using KioskRebornLib;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KioskReborn
{
    public partial class WallpaperWindow : Window
    {
        public WallpaperWindow()
        {
            #if !DEBUG
                Shell.Hide();
            #endif

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

            InitializeComponent();

            ImageBrush brush = new ImageBrush();
            brush.ImageSource = ConvertImage(Image.FromFile(settings.Background));
            Background = brush;

            SendWindow(this, true);
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TaskBarWindow taskBarWindow = new TaskBarWindow();
            taskBarWindow.Show();
        }

        private void OnClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            #if !DEBUG
                Shell.Show();
            #endif
        }

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X,
            int Y,
            int cx,
            int cy,
            uint uFlags);

        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        static readonly IntPtr HWND_TOP = new IntPtr(0);

        static void SendWindow(Window window, bool back)
        {
            IntPtr hWnd = new WindowInteropHelper(window).Handle;

            if (back)
            {
                SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
            }
            else {
                SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
            }
        }
    }
}
