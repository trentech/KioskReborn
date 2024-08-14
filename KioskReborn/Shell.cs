using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KioskReborn
{
    public class Shell
    {
        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(int hWnd, uint Msg, int wParam, int lParam);

        public static void Hide()
        {
            const int WM_USER = 0x0400;
            try
            {
                int ptr = FindWindow("Shell_TrayWnd", null);
                PostMessage(ptr, WM_USER + 436, 0, 0);

                do
                {
                    ptr = FindWindow("Shell_TrayWnd", null);

                    if (ptr == 0)
                    {
                        break;
                    }

                    Thread.Sleep(1000);
                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} {1}", ex.Message, ex.StackTrace);
            }
        }

        public static void Show()
        {
            Process[] explorer = Process.GetProcessesByName("explorer");

            foreach (Process p in explorer)
            {
                p.Kill();
            }
            Thread.Sleep(1000);

            Process process = new Process();
            process.StartInfo.FileName = string.Format(@"{0}\{1}", Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");
            process.StartInfo.UseShellExecute = true;
            process.Start();
        }
    }
}
