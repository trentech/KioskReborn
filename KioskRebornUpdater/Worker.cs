using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Win32;
using System.Security;

namespace KioskRebornUpdater
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("Starting KioskReborn Updater Service.");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("Stopping KioskReborn Updater Service.");

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {


            while (!stoppingToken.IsCancellationRequested)
            {
                Log.Information("Checking for updates");

                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KioskReborn.exe"));
                Version currentVersion = Version.Parse(versionInfo.ProductVersion);

                Log.Information("Current Version: " + currentVersion);

                string update = await CheckForUpdate(currentVersion);

                if (update != string.Empty)
                {
                    bool relaunchApp = Process.GetProcessesByName("KioskReborn").Length != 0;

                    HttpClient httpClient = new HttpClient();

                    string path = Path.Combine(Path.GetTempPath(), "KioskReborn_Setup.exe");

                    using (var stream = await httpClient.GetStreamAsync(update))
                    {
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            Log.Information("Downloading update");
                            stream.CopyTo(fileStream);
                        }
                    }

                    Log.Information("Installing update");

                    Process process = new Process();

                    process.StartInfo.FileName = path;
                    process.StartInfo.Arguments = "/VERYSILENT";

                    process.Start();

                    process.WaitForExit();

                    File.Delete(path);

                    if (relaunchApp) 
                    {
                        LaunchApplication();
                    }
                } else
                {
                    Log.Information("KioskReborn is up to date!");
                }

                var now = DateTime.Now;
                var hours = 23 - now.Hour;
                var minutes = 59 - now.Minute;
                var seconds = 59 - now.Second;
                var secondsTillMidnight = hours * 3600 + minutes * 60 + seconds;

                // wait till midnight
                await Task.Delay(TimeSpan.FromSeconds(secondsTillMidnight), stoppingToken);
                //await Task.Delay(1000 * 60, stoppingToken);
            }
        }

        private void LaunchApplication()
        {
            string domainName = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", "DefaultDomainName", string.Empty);
            string userName = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", "DefaultUserName", string.Empty);
            SecureString password = new NetworkCredential("", (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon", "DefaultPassword", string.Empty)).SecurePassword;

            if (userName == string.Empty || password.Length == 0)
            {
                Log.Error("Cannot relaunch application. Autologin is not configured");
            }
            else
            {
                Process process = new Process();

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KioskReborn.exe");

                if (domainName != string.Empty)
                {
                    process.StartInfo.Domain = domainName;
                }

                process.StartInfo.UserName = userName;
                process.StartInfo.Password = password;
                process.Start();
            }
        }

        private async Task<string> CheckForUpdate(Version currentVersion)
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
