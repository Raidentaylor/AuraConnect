using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AuraConnect.Core
{
    /// <summary>
    /// The Aura Connect extension methods
    /// </summary>
    public static class AuraConnectExtensions
    {
        /// <summary>
        /// Configures Aura Connect
        /// </summary>
        /// <param name="builder">The host builder</param>
        /// <param name="auraConnectDelegate">The action to execute upon configuring</param>
        /// <returns>The original host builder for chaining</returns>
        public static IHostBuilder ConfigureAuraConnect(this IHostBuilder builder, Action<IAuraConnectService> auraConnectDelegate)
        {
            var auraConnect = new AuraConnectService();

            auraConnectDelegate(auraConnect);

            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<IAuraConnectService>(auraConnect);
            });
        }
    }
}
