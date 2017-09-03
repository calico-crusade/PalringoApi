namespace PalringoApi.Networking.PacketHandling.Handlers
{
    /// <summary>
    /// Handles a Ping message
    /// </summary>
    public class PingHandler : PacketHandler
    {
        /// <summary>
        /// The command "P"
        /// </summary>
        public override string Command => "P";

        /// <summary>
        /// Handles the packet's process
        /// </summary>
        /// <param name="packet"></param>
        public override void Process(Packet packet)
        {
            Client.WritePacket(PacketTemplates.Ping());
        }
    }
}
