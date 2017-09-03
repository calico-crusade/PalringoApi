using System;

namespace PalringoApi.Networking.PacketHandling.PacketMap
{
    /// <summary>
    /// The attribute for mapping packets properties to different Attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PacketHeader : Attribute
    {
        /// <summary>
        /// The key of the packet to look for
        /// </summary>
        public string Key { private set; get; }

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="key">The key to look for</param>
        public PacketHeader(string key)
        {
            Key = key;
        }
    }
}
