using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AuraConnect.Core;

namespace AuraConnect
{
    /// <summary>
    /// The aura connect service program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Allocates a console window
        /// </summary>
        [DllImport("kernel32")]
        public static extern void AllocConsole();

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
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAuraConnect(auraConnect =>
                {
                    auraConnect.UseAsus();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddEventLog();
                })
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                });
        }
    }
}
