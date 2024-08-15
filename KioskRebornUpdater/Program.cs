using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using KioskRebornLib;
using System.Diagnostics;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Security;

namespace KioskRebornTask
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().MinimumLevel.Override("Microsoft", LogEventLevel.Information).Enrich.FromLogContext()
                .WriteTo.File(Settings.PATH + @"\update.log", outputTemplate: "[{Timestamp:MM/dd/yyyy h:mm tt}] [{Level}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Month, retainedFileCountLimit: 2)
                .WriteTo.Console(outputTemplate: "[{Timestamp:MM/dd/yyyy h:mm:ss tt}] [{Level:u3}] {Message:lj}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
                .WriteTo.EventLog("KioskRebornUpdater", manageEventSource: true)
                .CreateLogger();
            }
            catch
            {
                Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().MinimumLevel.Override("Microsoft", LogEventLevel.Information).Enrich.FromLogContext()
                .WriteTo.File(Settings.PATH + @"\update.log", outputTemplate: "[{Timestamp:MM/dd/yyyy h:mm tt}] [{Level}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Month, retainedFileCountLimit: 2)
                .WriteTo.Console(outputTemplate: "[{Timestamp:MM/dd/yyyy h:mm:ss tt}] [{Level:u3}] {Message:lj}{NewLine}{Exception}", theme: AnsiConsoleTheme.Code)
                .WriteTo.EventLog(source: "Application")
                .CreateLogger();
            }

            Log.Information("Checking for updates");

            string appPath = new FileInfo(AppDomain.CurrentDomain.BaseDirectory).Directory.Parent.FullName;

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(appPath, "KioskReborn.exe"));
            Version currentVersion = Version.Parse(versionInfo.ProductVersion);

            Log.Information("Current Version: " + currentVersion);

            string update = CheckForUpdate(currentVersion).Result;

            if (update != string.Empty)
            {
                bool relaunchApp = Process.GetProcessesByName("KioskReborn").Length != 0;

                HttpClient httpClient = new HttpClient();

                string path = Path.Combine(Path.GetTempPath(), "KioskReborn_Setup.exe");

                using (var stream = httpClient.GetStreamAsync(update).Result)
                {
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        Log.Information("Downloading update");
                        stream.CopyTo(fileStream);
                    }
                }

                Log.Information("Installing update");

                Process[] explorer = Process.GetProcessesByName("KioskReborn");

                foreach (Process p in explorer)
                {
                    p.Kill();
                }

                Thread.Sleep(1000);

                Process process = new Process();

                process.StartInfo.FileName = path;
                process.StartInfo.Arguments = "/SILENT";

                process.Start();

                process.WaitForExit();

                File.Delete(path);

                if (relaunchApp)
                {
                    RestartComputer();
                }
            }
            else
            {
                Log.Information("KioskReborn is up to date!");
            }
        }

        static void RestartComputer()
        {
            ProcessStartInfo process = new ProcessStartInfo();

            process.FileName = "cmd";
            process.WindowStyle = ProcessWindowStyle.Hidden;
            process.Arguments = "/C shutdown -f -r -t 0";

            Process.Start(process);
        }

        static void LaunchApplication(string path)
        {
            Log.Information("Relaunching applications");

            string domainName = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", "DefaultDomainName", string.Empty);
            string userName = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", "DefaultUserName", string.Empty);
            SecureString password = new NetworkCredential("", (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", "DefaultPassword", string.Empty)).SecurePassword;

            if (userName == string.Empty || password.Length == 0)
            {
                Log.Error("Cannot relaunch application. Autologin is not configured");
            }
            else
            {
                ProcessStartInfo process = new ProcessStartInfo();

                process.UseShellExecute = false;
                process.FileName = Path.Combine(path, "KioskReborn.exe");

                if (domainName != string.Empty)
                {
                    process.Domain = domainName;
                }

                process.WorkingDirectory = path;
                process.UserName = userName;
                process.Password = password;
                Process.Start(process);
            }
        }
        static async Task<string> CheckForUpdate(Version currentVersion)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("MyApplication", "1"));

            var contentsUrl = $"https://api.github.com/repos/trentech/KioskReborn/contents/KioskRebornSetup?ref=master";
            var contentsJson = await httpClient.GetStringAsync(contentsUrl);
            var contents = (JArray)JsonConvert.DeserializeObject(contentsJson);

            foreach (var file in contents)
            {
                var name = (string)file["name"];

                if (name.StartsWith("KioskReborn_Setup_"))
                {
                    Version latestVersion = Version.Parse(name.Replace("KioskReborn_Setup_", "").Replace(".exe", ""));

                    if (currentVersion < latestVersion)
                    {
                        Log.Information("New Version Available: " + latestVersion);

                        return (string)file["download_url"];
                    }

                    return string.Empty;
                }
            }

            return string.Empty;
        }
    }
}
