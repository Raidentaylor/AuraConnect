using System;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Razer.Chroma.Broadcast;
using AuraConnect.Core;

namespace AuraConnect
{
    /// <summary>
    /// The aura connect worker
    /// </summary>
    public class Worker : BackgroundService
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger<Worker> _logger;

        /// <summary>
        /// The aura connect service
        /// </summary>
        private readonly IAuraConnectService _auraConnect;

        /// <summary>
        /// The razer broadcast api
        /// </summary>
        private readonly RzChromaBroadcastAPI _api;

        /// <summary>
        /// The health check timer
        /// </summary>
        private System.Timers.Timer _healthCheckTimer;

        /// <summary>
        /// Creates the worker
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="auraConnect">The aura connect service</param>
        public Worker(ILogger<Worker> logger, IAuraConnectService auraConnect)
        {
            _logger = logger;
            _auraConnect = auraConnect;
            _api = new RzChromaBroadcastAPI();
            _api.ConnectionChanged += Api_ConnectionChanged;
            _api.ColorChanged += Api_ColorChanged;
            _healthCheckTimer = new System.Timers.Timer(15000);
            _healthCheckTimer.Elapsed += HealthCheckTimer_Elapsed;
        }

        /// <summary>
        /// Executes the worker
        /// </summary>
        /// <param name="stoppingToken">The stopping token</param>
        /// <returns>A task</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1, stoppingToken);

            _auraConnect.Initialize();

            _api.Init(Guid.Parse("e6bef332-95b8-76ec-a6d0-9f402bad244c"));

            foreach (var provider in _auraConnect.DeviceProviders)
            {
                provider.RequestControl();

                foreach (var device in provider.Devices)
                {
                    _logger.LogInformation("Device Found: " + provider.Name + " - " + device.Name);
                }
            }

            _healthCheckTimer.Start();

            _logger.LogInformation("Aura Connect started successfully...");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            _api.UnInit();
            _healthCheckTimer.Stop();
        }

        /// <summary>
        /// Occurs when the connection status to the Razer Chroma Broadcast API changes
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The arguments</param>
        private void Api_ConnectionChanged(object sender, RzChromaBroadcastConnectionChangedEventArgs e)
        {
            _logger.LogInformation(e.Connected ? "Razer Chroma Broadcast API connected" : "Razer Chroma Broadcast API disconnected");
        }

        /// <summary>
        /// Occurs when the connection status to the Razer Chroma Broadcast API changes
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The arguments</param>
        private void Api_ColorChanged(object sender, RzChromaBroadcastColorChangedEventArgs e)
        {
            var currentColor = 0;

            foreach (var deviceProvider in _auraConnect.DeviceProviders)
            {
                foreach (var device in deviceProvider.Devices)
                {
                    foreach (var light in device.Lights)
                    {
                        light.Color = e.Colors[currentColor];
                        currentColor++;

                        if (currentColor == e.Colors.Length)
                            currentColor = 0;
                    }

                    device.ApplyLights();
                }
            }
        }

        /// <summary>
        /// Occurs during a health check cycle
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The arguments</param>
        private void HealthCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (var provider in _auraConnect.DeviceProviders)
            {
                provider.PerformHealthCheck();
                provider.RequestControl();
            }
        }
    }
}
