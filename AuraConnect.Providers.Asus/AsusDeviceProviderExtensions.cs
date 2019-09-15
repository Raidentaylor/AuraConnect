using AuraConnect.Providers.Asus;

namespace AuraConnect.Core
{
    /// <summary>
    /// The ASUS Aura Connect extension methods
    /// </summary>
    public static class AsusDeviceProviderExtensions
    {
        /// <summary>
        /// Uses the ASUS device provider
        /// </summary>
        /// <param name="auraConnect">The Aura Connect instance to add the provider to</param>
        public static void UseAsus(this IAuraConnectService auraConnect)
        {
            auraConnect.AddProvider(new AsusDeviceProvider());
        }
    }
}
