namespace PalringoApi.Networking.PacketHandling.Handlers
{
    /// <summary>
    /// Handles a "Ghosted" packet
    /// </summary>
    public class GhostedHandler : PacketHandler
    {
        /// <summary>
        /// "GHOSTED"
        /// </summary>
        public override string Command => "GHOSTED";

        /// <summary>
        /// What happens when the packet is processed
        /// </summary>
        /// <param name="packet"></param>
        public override void Process(Packet packet)
        {
            Bot.On.Trigger("lf", "You have been signed on else where.");
            Bot.On.Trigger("gh");
        }
    }
}
