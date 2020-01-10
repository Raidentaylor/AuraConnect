using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RGBKit.Core;

namespace AuraConnect
{
    /// <summary>
    /// The Aura Connect service program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Allocates a console window
        /// </summary>
        [DllImport("kernel32")]
        private static extern void AllocConsole();

        /// <summary>
        /// The program entry point
        /// </summary>
        /// <param name="args">The command line arguments</param>
        public static void Main(string[] args)
        {
            var mutex = new Mutex(true, "AuraConnect", out var result);

            if (!result)
            {
                return;
            }

            if (Debugger.IsAttached || args.Length > 0 && args[0] == "--console")
            {
                AllocConsole();
            }

            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates the host builder
        /// </summary>
        /// <param name="args">The arguments</param>
        /// <returns>The host builder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AuraConnect\\logs");

            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);

            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureRGBKit(rgbKit =>
                {
                    rgbKit.UseAura();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddEventLog();
                    logging.AddFile(Path.Combine(logFolder, $"AuraConnect.log"), append: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
        }
    }
}
