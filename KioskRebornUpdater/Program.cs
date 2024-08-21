using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using KioskRebornLib;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Octokit;

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

            Log.Information("Checking for updates");

            string appPath = new FileInfo(AppDomain.CurrentDomain.BaseDirectory).Directory.Parent.FullName;

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(appPath, "KioskReborn.exe"));

            Version currentVersion = Version.Parse(versionInfo.ProductVersion);

            Log.Information("Current Version: " + currentVersion);

            string update = CheckForUpdate(currentVersion).Result;

            if (update != string.Empty)
            {
                HttpClient httpClient = new HttpClient();

                string path = Path.Combine(Path.GetTempPath(), "KioskReborn_Setup.exe");

                using (var stream = httpClient.GetStreamAsync(update).Result)
                {
                    using (var fileStream = new FileStream(path, System.IO.FileMode.Create))
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

                Log.Information("Update complete");

                File.Delete(path);

                RestartComputer();
            }
            else
            {
                Log.Information("KioskReborn is up to date!");
            }
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


        public async Task CheckForUpdates(GitHubClient gitHubClient)
        {
            try
            {
                var latestRelease = await gitHubClient.Repository.Release.GetLatest("trentech", "KioskReborn");
                string latestVersion = latestRelease.TagName;

                if (IsUpdateAvailable(latestVersion))
                {
                    bool IsDownloaded = DownloadAndUpdate(latestRelease.Assets[0].BrowserDownloadUrl);
                }
                else
                {
                    Console.WriteLine("You are already using the latest version.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving latest version from GitHub: " + ex.Message);
            }
        }

        private bool IsUpdateAvailable(string latestVersion)
        {
            string appPath = new FileInfo(AppDomain.CurrentDomain.BaseDirectory).Directory.Parent.FullName;

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(appPath, "KioskReborn.exe"));

            Version current = Version.Parse(versionInfo.ProductVersion);

            if (!string.IsNullOrEmpty(latestVersion))
            {
                Version latest = new Version(latestVersion);
                return latest > current;
            }
            return false;
        }

        private bool DownloadAndUpdate(string downloadUrl)
        {
            try
            {
                HttpClient httpClient = new HttpClient();

                string path = Path.Combine(Path.GetTempPath(), "KioskReborn_Setup.exe");

                using (var stream = httpClient.GetStreamAsync(downloadUrl).Result)
                {
                    using (var fileStream = new FileStream(path, System.IO.FileMode.Create))
                    {
                        Log.Information("Downloading update");
                        stream.CopyTo(fileStream);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
