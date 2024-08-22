using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using KioskRebornLib;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Octokit;
using System.IO;

namespace KioskRebornTask
{
    #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    #pragma warning disable CS8602 // Dereference of a possibly null reference.
    #pragma warning disable CS8604 // Possible null reference argument.
    #pragma warning disable CS8603 // Possible null reference return.
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

            Task<string> task = CheckForUpdates();
            task.Wait();
    
            string update = task.Result;

            if (update != string.Empty)
            {
                if (DownloadUpdate(update))
                {
                    if (InstallUpdate())
                    {
                        Log.Information("Update Complete");
                        RestartComputer();
                    }
                }
            }
        }

        private static async Task<string> CheckForUpdates()
        {
            Log.Information("Checking for updates");

            try
            {
                GitHubClient client = new GitHubClient(new Octokit.ProductHeaderValue("KioskReborn"));

                var latestRelease = await client.Repository.Release.GetLatest("trentech", "KioskReborn");

                string latestVersion = latestRelease.TagName;

                if (IsUpdateAvailable(latestVersion))
                {
                    return latestRelease.Assets[0].BrowserDownloadUrl;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error retrieving latest version from GitHub: " + ex.Message);
            }

            return String.Empty;
        }

        private static bool IsUpdateAvailable(string latestVersion)
        {
            string appPath = new FileInfo(AppDomain.CurrentDomain.BaseDirectory).Directory.Parent.FullName;

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(appPath, "KioskReborn.exe"));

            Version current = Version.Parse(versionInfo.ProductVersion);

            if (!string.IsNullOrEmpty(latestVersion))
            {
                Version latest = new Version(latestVersion);

                if (latest > current)
                {
                    Log.Information($"Current Version {current}");
                    Log.Information($"Latest Version {latestVersion}");
                    return true;
                }
                else
                {
                    Log.Information("KioskReborn is up to date");
                }
            }

            return false;
        }

        private static bool DownloadUpdate(string url)
        {
            Log.Information($"Download update: {url}");

            try
            {
                HttpClient httpClient = new HttpClient();

                string path = Path.Combine(Path.GetTempPath(), "KioskReborn_Setup.exe");

                using (var stream = httpClient.GetStreamAsync(url).Result)
                {
                    using (var fileStream = new FileStream(path, System.IO.FileMode.Create))
                    {
                        stream.CopyTo(fileStream);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error downloading update: " + ex.Message);
            }

            return false;
        }

        private static bool InstallUpdate()
        {
            Log.Information("Installing update");

            try
            {
                string path = Path.Combine(Path.GetTempPath(), "KioskReborn_Setup.exe");

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

                return true;
            }
            catch (Exception ex)
            {
                Log.Error("Error installing update: " + ex.Message);
            }

            return false;
        }

        static void RestartComputer()
        {
            Log.Information("Restarting computer");

            ProcessStartInfo process = new ProcessStartInfo();

            process.FileName = "cmd";
            process.WindowStyle = ProcessWindowStyle.Hidden;
            process.Arguments = "/C shutdown -f -r -t 0";

            Process.Start(process);
        }
    }
}
