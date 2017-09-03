namespace PalringoApi.Networking.PacketHandling.Handlers
{
    /// <summary>
    /// The subprofile handler
    /// </summary>
    public class SubProfileHandler : PacketHandler
    {
        /// <summary>
        /// The Sub profile command
        /// </summary>
        public override string Command { get; } = "SUB PROFILE";

        /// <summary>
        /// Processes the packet
        /// </summary>
        /// <param name="packet"></param>
        public override void Process(Packet packet)
        {
            Bot.Information.Parse(Bot, packet);
        }
    }
}
