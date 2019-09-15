using System.Collections.Generic;

namespace AuraConnect.Core
{
    /// <summary>
    /// The Aura Connect service definition
    /// </summary>
    public interface IAuraConnectService
    {
        /// <summary>
        /// The list of device providers
        /// </summary>
        IEnumerable<IDeviceProvider> DeviceProviders { get; }

        /// <summary>
        /// Adds a device provider to the service
        /// </summary>
        /// <param name="provider">The provider to add</param>
        void AddProvider(IDeviceProvider provider);

        /// <summary>
        /// Initializes the Aura Connect service
        /// </summary>
        void Initialize();
    }
}
