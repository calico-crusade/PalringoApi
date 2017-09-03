using System;

namespace PalringoApi.Networking.PacketHandling.PacketMap
{
    /// <summary>
    /// Marks the property as the Payload of the packet
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PacketPayload : Attribute
    {
    }
}
