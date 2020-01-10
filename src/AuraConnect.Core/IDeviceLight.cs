using System.Drawing;

namespace AuraConnect.Core
{
    /// <summary>
    /// The device light interface
    /// </summary>
    public interface IDeviceLight
    {
        /// <summary>
        /// The device lights color
        /// </summary>
        Color Color { get; set; }
    }
}
