namespace PalringoApi.Networking.PacketHandling.Handlers
{
    /// <summary>
    /// What happens on a Balance Query packet
    /// </summary>
    public class BalanceQueryHandler : PacketHandler
    {
        /// <summary>
        /// The Command "BALANCE QUERY RESULT"
        /// </summary>
        public override string Command { get; } = "BALANCE QUERY RESULT";

        /// <summary>
        /// What happens when the packet is processed.
        /// </summary>
        /// <param name="packet"></param>
        public override void Process(Packet packet)
        {
            Bot.Information.Parse(Bot, packet);
            Bot.On.Trigger("ls");
        }
    }
}
