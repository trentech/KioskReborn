using System.Windows;

namespace KioskReborn
{
    public class RestartApp
    {
        public static void Restart()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
