namespace PalringoApi.Networking.PacketHandling
{
    /// <summary>
    /// Interface for handling packets
    /// </summary>
    public interface IPacketHandler
    {
        /// <summary>
        /// The command of the packet
        /// </summary>
        string Command { get; }

        /// <summary>
        /// The bot instance to attach to
        /// </summary>
        PalBot Bot { get; set; }

        /// <summary>
        /// The client instance for sending packets
        /// </summary>
        Client Client { get; set; }

        /// <summary>
        /// The method to process packets
        /// </summary>
        /// <param name="packet"></param>
        void Process(Packet packet);
    }
}
