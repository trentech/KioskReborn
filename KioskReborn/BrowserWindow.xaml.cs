using KioskRebornLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KioskReborn
{
    public partial class BrowserWindow : Window
    {
        public BrowserWindow(string URL)
        {
            InitializeComponent();

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

            List<Settings.Favorite> favorites = settings.Favorites;

            foreach (Settings.Favorite favorite in favorites)
            {
                TextBlock textBlock = new TextBlock();

                textBlock.FontSize = 14;
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.FontSize = 14;
                textBlock.FontFamily = new FontFamily("Calibri");
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.Text = favorite.Name;

                Style style = Application.Current.FindResource("BrowserTextFavorite") as Style;
                textBlock.Style = style;

                Button button = new Button();

                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.VerticalAlignment = VerticalAlignment.Stretch;
                button.BorderThickness = new Thickness(2, 2, 2, 2);

                button.Click += (sender, args) =>
                {
                    webView.Source = new Uri(favorite.URL);
                };

                style = Application.Current.FindResource("BrowserButtonFavorite") as Style;
                button.Style = style;

                button.Content = textBlock;

                ToolBar.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(favorite.Width) });
                ToolBar.Children.Add(button);
                Grid.SetColumn(button, ToolBar.ColumnDefinitions.Count - 1);
            }
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
    }
}
