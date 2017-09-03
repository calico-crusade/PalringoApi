namespace PalringoApi.Networking.PacketHandling.Handlers
{
    /// <summary>
    /// Handles Response Packets (doesn't actually do anything)
    /// </summary>
    public class ResponseHandler : PacketHandler
    {

        /// <summary>
        /// The Command "RESPONSE"
        /// </summary>
        public override string Command => "RESPONSE";

        /// <summary>
        /// The packet that has been processed
        /// </summary>
        /// <param name="packet"></param>
        public override void Process(Packet packet)
        {
        }
    }
}
