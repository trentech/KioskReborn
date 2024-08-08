using KioskRebornLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace KioskReborn
{
    public partial class PasswordWindow : Window
    {
        public PasswordWindow()
        {
            InitializeComponent();
            Left = (SystemParameters.WorkArea.Width / 2) - (Width / 2);
            Top = (SystemParameters.WorkArea.Height / 2) - (Height / 2);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            Execute();
        }

        private void OnKeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Execute();
            }
        }

        private void Execute()
        {
            if (TextPasswd.Password == File.ReadAllText(System.IO.Path.Combine(Settings.PATH, "passwd")))
            {
                Process process = new Process();
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = @"cmd.exe";
                processStartInfo.Arguments = "/c \"" + System.IO.Path.Combine(AppContext.BaseDirectory, @"Resources\elevate.bat") + "\" cmd.exe"; //FIX PATH TO ELEVATE
                processStartInfo.UseShellExecute = true;
                process.StartInfo = processStartInfo;

                ThreadStart threadStart = new ThreadStart(() => process.Start());
                Thread thread = new Thread(threadStart);
                thread.Start();

                this.Close();
            }
            else
            {
                MessageBox.Show("Password is incorrect");
            }
        }
    }
}
