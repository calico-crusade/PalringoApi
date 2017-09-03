namespace PalringoApi.Networking.PacketHandling.Handlers
{
    /// <summary>
    /// Handles sub profile query results (Doesn't do anything)
    /// </summary>
    public class SubProfileQueryResultHandler : PacketHandler
    {
        /// <summary>
        /// The command
        /// </summary>
        public override string Command => "SUB PROFILE QUERY RESULT";

        /// <summary>
        /// The processor
        /// </summary>
        /// <param name="packet"></param>
        public override void Process(Packet packet)
        {

        }
    }
}
