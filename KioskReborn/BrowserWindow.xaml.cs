using KioskRebornLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace KioskReborn
{
    public partial class BrowserWindow : Window
    {
        public BrowserWindow(string URL)
        {
            InitializeComponent();

            this.DataContext = this;
           UpdateTouchStatus();

            string cache = Path.Combine(Settings.PATH, "cache");

            if (!Directory.Exists(cache))
            {
                Directory.CreateDirectory(cache);
            }

            Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER", cache);

            Width   = SystemParameters.PrimaryScreenWidth;
            Height  = SystemParameters.PrimaryScreenHeight;

            webView.Width = Width;
            webView.Height = Height - 50;

            webView.Source = new Uri(URL);

            Settings settings = Settings.Get();

            TextBlock textBlock = new TextBlock();

            textBlock.TextWrapping = TextWrapping.Wrap;
            textBlock.TextAlignment = TextAlignment.Center;
            textBlock.FontSize = 20;
            textBlock.FontFamily = new FontFamily("Calibri");
            textBlock.FontWeight = FontWeights.Bold;
            textBlock.Text = "Bookmarks";

            MenuItem bookmarksMenu = new MenuItem();
            bookmarksMenu.Header = textBlock;
            bookmarksMenu.Height = 46;
            bookmarksMenu.BorderThickness = new Thickness(0);

            Style style = Application.Current.FindResource("BrowserMenuItem") as Style;
            bookmarksMenu.Style = style;

            List<Settings.Favorite> favorites = settings.Favorites;

            foreach (Settings.Favorite favorite in favorites)
            {
                textBlock = new TextBlock();

                textBlock.FontSize = 20;
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.FontFamily = new FontFamily("Calibri");
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.Text = favorite.Name;

                MenuItem menuItem = new MenuItem();
                menuItem.Header = textBlock;
                menuItem.Height = 50;
                menuItem.BorderThickness = new Thickness(0);

                menuItem.Click += (sender, args) => 
                {
                    webView.Source = new Uri(favorite.URL);
                };

                style = Application.Current.FindResource("BrowserMenuItem") as Style;
                menuItem.Style = style;

                bookmarksMenu.Items.Add(menuItem);
                bookmarksMenu.Items.Add(new Separator());
            }

            BookmarksMenu.Items.Add(bookmarksMenu);

            Activate();
            Focus();
        }

        private void OnBack(object sender, EventArgs e)
        {
            webView.GoBack();
        }

        private void OnForward(object sender, EventArgs e)
        {
            webView.GoForward();
        }

        private void OnExit(object sender, EventArgs e)
        {
            if (Settings.Get().Browser.AllowExit)
            {
                this.Close();
            }
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            webView.Reload();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool touchEnabled;
        public bool TouchEnabled { get => this.touchEnabled; set { this.touchEnabled = value; OnPropertyChanged(); } }

        private void UpdateTouchStatus()
        {
            if (HasTouchInput())
            {
                this.TouchEnabled = true;
            }
            else
            {
                this.TouchEnabled = false;
            }
        }

        public bool HasTouchInput()
        {
            foreach (TabletDevice tabletDevice in Tablet.TabletDevices)
            {
                if (tabletDevice.Type == TabletDeviceType.Touch)
                {
                    return true;
                }
            }

            return false;
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

        static readonly IntPtr HWND_TOP = new IntPtr(0);

        static void SendWpfWindowForward(Window window)
        {
            var hWnd = new WindowInteropHelper(window).Handle;
            SetWindowPos(hWnd, HWND_TOP, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        }
    }
}
