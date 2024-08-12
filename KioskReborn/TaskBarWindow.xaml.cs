using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Drawing;
using Image = System.Windows.Controls.Image;
using System.Threading;
using System.Windows.Media.Media3D;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Media;
using System.Drawing.Imaging;
using KioskRebornLib;
using System.ComponentModel;

namespace KioskReborn
{
    public partial class TaskBarWindow : Window
    {
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int nCmdShow);
        [DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);

        DispatcherTimer timer = new DispatcherTimer();

        public TaskBarWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            UpdateTouchStatus();

            Shutdown.ToolTip = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

            Clock.Content = DateTime.Now.ToString("hh:mm:ss tt");

            timer.Tick += new EventHandler(OnTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            Settings settings = Settings.Get();

            List<ButtonSettings> buttons = ButtonSettings.getButtons();

            if (settings.Browser.Enable)
            {
                Button button = new Button();
                button.Name = "Browser";
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.VerticalAlignment = VerticalAlignment.Stretch;
                button.BorderThickness = new Thickness(0, 0, 0, 0);
                button.ToolTip = "Web Browser";

                Style style = Application.Current.FindResource("TaskbarButton") as Style;
                button.Style = style;
                button.Content = Application.Current.FindResource("Browser") as Image;

                button.Click += (sender, args) =>
                {
                    bool isOpen = false;
                    foreach (Window w in Application.Current.Windows)
                    {
                        if (w.Name == "Browser")
                        {
                            isOpen = true;
                            w.Activate();
                        }
                    }

                    if (!isOpen)
                    {
                        BrowserWindow browser = new BrowserWindow(settings.Browser.URL);
                        browser.Show();
                    }
                };

                TaskbarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
                TaskbarGrid.Children.Add(button);
                Grid.SetColumn(button, TaskbarGrid.ColumnDefinitions.Count - 1);
                Taskbar.Width = Taskbar.Width + 50;
            }

            foreach (ButtonSettings buttonSettings in buttons)
            {
                if (buttonSettings.name.Equals("UniqueNameHere"))
                {
                    continue;
                }

                Button button = new Button();
                button.Name = buttonSettings.name.Replace(" ", "_");
                button.BorderThickness = new Thickness(0, 0, 0, 0);
                button.ToolTip = buttonSettings.name;

                Style style = Application.Current.FindResource("TaskbarAppButton") as Style;
                button.Style = style;

                Image image = Application.Current.FindResource("NoIcon") as Image;

                button.Content = image.Source;

                button.Click += (sender, args) =>
                {
                    string processName = Environment.ExpandEnvironmentVariables(buttonSettings.executable);

                    if (processName.Contains("\\"))
                    {
                        processName = buttonSettings.executable.Substring(buttonSettings.executable.LastIndexOf("\\")).Replace("\\", "").Replace(".exe", "");
                    }

                    Process[] processes = Process.GetProcessesByName(processName);

                    if (processes.Length != 0)
                    {
                        foreach (Process process in processes)
                        {
                            IntPtr handle = process.MainWindowHandle;

                            if (IsIconic(handle))
                            {
                                ShowWindow(handle, 9);
                                ShowWindow(handle, 3);
                            }

                            SetForegroundWindow(handle);
                        }
                    }
                    else
                    {
                        Execute(buttonSettings);
                    }
                };

                string icon = Environment.ExpandEnvironmentVariables(buttonSettings.icon);

                if (!icon.Equals("none"))
                {
                    if(File.Exists(icon))
                    {
                        image = new Image();

                        if (icon.ToLower().EndsWith(".exe"))
                        {
                            try
                            {
                                image.Source = Imaging.CreateBitmapSourceFromHBitmap(System.Drawing.Icon.ExtractAssociatedIcon(icon).ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

                                button.Content = image;
                            }
                            catch { }
                        }
                        else if (icon.ToLower().EndsWith(".png"))
                        {
                            try
                            {
                                Stream imageStreamSource = new FileStream(icon, FileMode.Open, FileAccess.Read, FileShare.Read);
                                PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                                BitmapSource bitmapSource = decoder.Frames[0];

                                image.Source = bitmapSource;

                                button.Content = image;
                            }
                            catch { }
                        }
                        else if (icon.ToLower().EndsWith(".ico"))
                        {
                            image.Source = Imaging.CreateBitmapSourceFromHBitmap(new Icon(icon).ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            button.Content = image;
                        }
                    }
                }

                TaskbarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });
                TaskbarGrid.Children.Add(button);
                Grid.SetColumn(button, TaskbarGrid.ColumnDefinitions.Count - 1);
                Taskbar.Width = Taskbar.Width + 50;

                if (settings.Browser.AutoStart)
                {
                    BrowserWindow browser = new BrowserWindow(settings.Browser.URL);
                    browser.Show();
                }
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            Clock.Content = DateTime.Now.ToString("hh:mm:ss tt");
        }

        private void OnKeyboard(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("TouchKeyboard");

            if (processes.Length != 0)
            {
                foreach (Process p in processes)
                {
                    IntPtr handle = p.MainWindowHandle;

                    if (IsIconic(handle))
                    {
                        ShowWindow(handle, 9);
                        ShowWindow(handle, 3);
                    }

                    SetForegroundWindow(handle);
                }
            }
            else
            {
                Process process = Process.Start(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TouchKeyboard.exe"));
                IntPtr handle = process.MainWindowHandle;

                ShowWindow(handle, 9);
                ShowWindow(handle, 3);
                SetForegroundWindow(handle);
            }
        }

        private void OnClock(object sender, EventArgs e)
        {
            bool isOpen = false;
            foreach (Window w in Application.Current.Windows)
            {
                if (w.Name == "Calendar")
                {
                    w.Close();
                    isOpen = true;
                }
            }

            if (!isOpen)
            {
                CalendarWindow calendar = new CalendarWindow();
                calendar.Show();
            }
        }

        private void OnShell(object sender, EventArgs e)
        {
            bool isOpen = false;
            foreach (Window w in Application.Current.Windows)
            {
                if (w.Name == "Password")
                {
                    w.Activate();
                    isOpen = true;
                }
            }

            if (!isOpen)
            {
                PasswordWindow passwd = new PasswordWindow();
                passwd.Show();
            }
        }

        private void OnShutdown(object sender, EventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Are you sure you want to restart?", "Restart Kiosk?", MessageBoxButton.YesNo);
            if (dialogResult == MessageBoxResult.Yes)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.FileName = "shutdown";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.Arguments = "-r -t 0";

                try
                {
                    Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Execute(ButtonSettings buttonSettings)
        {
            if (buttonSettings.executable == "UWP")
            {
                ApplicationActivationManager appActiveManager = new ApplicationActivationManager();
                uint pid;
                appActiveManager.ActivateApplication(buttonSettings.arguments, null, ActivateOptions.None, out pid);
            }
            else
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = Environment.ExpandEnvironmentVariables(buttonSettings.executable);

                if (!buttonSettings.arguments.Equals("none"))
                {
                    processStartInfo.Arguments = Environment.ExpandEnvironmentVariables(buttonSettings.arguments);
                }

                try
                {
                    Process process = Process.Start(processStartInfo);
                    IntPtr handle = process.MainWindowHandle;

                    ShowWindow(handle, 9);
                    ShowWindow(handle, 3);
                    SetForegroundWindow(handle);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public enum ActivateOptions
        {
            None = 0x00000000,
            DesignMode = 0x00000001,
            NoErrorUI = 0x00000002,
            NoSplashScreen = 0x00000004,
        }

        [ComImport, Guid("2e941141-7f97-4756-ba1d-9decde894a3d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IApplicationActivationManager
        {
            IntPtr ActivateApplication([In] String appUserModelId, [In] String arguments, [In] ActivateOptions options, [Out] out UInt32 processId);
            IntPtr ActivateForFile([In] String appUserModelId, [In] IntPtr itemArray, [In] String verb, [Out] out UInt32 processId);
            IntPtr ActivateForProtocol([In] String appUserModelId, [In] IntPtr itemArray, [Out] out UInt32 processId);
        }

        [ComImport, Guid("45BA127D-10A8-46EA-8AB7-56EA9078943C")]
        class ApplicationActivationManager : IApplicationActivationManager
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            public extern IntPtr ActivateApplication([In] String appUserModelId, [In] String arguments, [In] ActivateOptions options, [Out] out UInt32 processId);
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            public extern IntPtr ActivateForFile([In] String appUserModelId, [In] IntPtr itemArray, [In] String verb, [Out] out UInt32 processId);
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            public extern IntPtr ActivateForProtocol([In] String appUserModelId, [In] IntPtr itemArray, [Out] out UInt32 processId);
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
    }
}