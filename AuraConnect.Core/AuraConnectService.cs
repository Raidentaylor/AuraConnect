using System.Linq;
using System.Collections.Generic;

namespace AuraConnect.Core
{
    /// <summary>
    /// The Aura Connect service
    /// </summary>
    public class AuraConnectService : IAuraConnectService
    {
        /// <summary>
        /// The list of device providers
        /// </summary>
        public IEnumerable<IDeviceProvider> DeviceProviders { get => _deviceProviders; }

        /// <summary>
        /// The device providers
        /// </summary>
        private List<IDeviceProvider> _deviceProviders;

        /// <summary>
        /// Creates a instance of the Aura Connect service
        /// </summary>
        public AuraConnectService()
        {
            _deviceProviders = new List<IDeviceProvider>();
        }

        /// <summary>
        /// Adds a device provider to the service
        /// </summary>
        /// <param name="provider">The provider to add</param>
        public void AddProvider(IDeviceProvider provider)
        {
            _deviceProviders.Add(provider);
        }

        /// <summary>
        /// Initializes the Aura Connect service
        /// </summary>
        public void Initialize()
        {
            foreach (var provider in DeviceProviders)
            {
                provider.Initialize();

                foreach (var device in provider.Devices)
                {
                    var duplicateDevices = provider.Devices.Where(d => d.Name == device.Name).ToArray();

                    if (duplicateDevices.Length > 1)
                    {
                        for (int i = 0; i < duplicateDevices.Length; i++)
                        {
                            duplicateDevices[i].Name = duplicateDevices[i].Name + " " + (i + 1);
                        }
                    }
                }
            }
        }
    }
}
