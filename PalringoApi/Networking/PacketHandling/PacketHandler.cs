namespace PalringoApi.Networking.PacketHandling
{
    /// <summary>
    /// Abstract implementation of <see cref="IPacketHandler"/>
    /// </summary>
    public abstract class PacketHandler : IPacketHandler
    {
        /// <summary>
        /// The instance of the Bot to attach to
        /// </summary>
        public PalBot Bot { get; set; }

        /// <summary>
        /// The packet sender client
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// The command of the packet
        /// </summary>
        public abstract string Command { get; }

        /// <summary>
        /// The process of the packet
        /// </summary>
        /// <param name="packet"></param>
        public abstract void Process(Packet packet);

        /// <summary>
        /// A short cut to the packet mapper <see cref="PacketMap.PacketMapper"/>
        /// </summary>
        /// <typeparam name="T">The object to attach to</typeparam>
        /// <param name="packet">The packet to map</param>
        /// <returns>The object that has been mapped.</returns>
        public T Map<T>(Packet packet)
        {
            return PacketMap.PacketMapper.Map<T>(packet);
        }
    }
}
