using KioskRebornLib;
using Microsoft.AspNetCore.Hosting.Server;
using Serilog;
using System;
using System.Diagnostics;
using System.Reflection;

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
            // calculate seconds till midnight
            var now = DateTime.Now;
            var hours = 23 - now.Hour;
            var minutes = 59 - now.Minute;
            var seconds = 59 - now.Second;
            var secondsTillMidnight = hours * 3600 + minutes * 60 + seconds;

            // wait till midnight
           // await Task.Delay(TimeSpan.FromSeconds(secondsTillMidnight), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                Log.Information("Checking for updates.");

                string application = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "KioskReborn.exe");

                if (!File.Exists(application))
                {
                    Log.Error("Could not located KioskReborn.exe");

                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                Settings settings = Settings.Get();

                string path = Path.Combine(settings.UpdateLocation, "KioskReborn_Setup.exe"); 

                if (!File.Exists(path))
                {
                    Log.Error("Could not located update files");

                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(application);
                Version currentVersion = Version.Parse(versionInfo.ProductVersion);

                versionInfo = FileVersionInfo.GetVersionInfo(path);
                Version updateVersion = Version.Parse(versionInfo.ProductVersion);

                Console.WriteLine("Current Version: " + currentVersion);
                Console.WriteLine("Available Version: " + updateVersion);

                if (currentVersion < updateVersion)
                {
                    Log.Information("Current Version: " + currentVersion);
                    Log.Information("Available Version: " + updateVersion);

                    Process[] processes = Process.GetProcessesByName("KioskReborn");

                    if (processes.Length != 0)
                    {
                        foreach (Process p in processes)
                        {
                            p.Kill();
                        }
                    }

                    Process process = new Process();

                    process.StartInfo.FileName = path;
                    process.StartInfo.Arguments = "/SILENT";
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                    process.Start();

                    process.WaitForExit();

                    if(!ProcessExtensions.StartProcessAsCurrentUser(application))
                    {
                        Log.Error("Failed to relaunch KioskReborn");
                    }
                }
                else
                {
                    Console.WriteLine("Up to date");
                }

                // wait 24 hours
                //await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
