namespace PalringoApi.Networking.PacketHandling.Handlers
{
    /// <summary>
    /// Handles when a packet fails to login
    /// </summary>
    public class LoginFailedHandler : PacketHandler
    {
        /// <summary>
        /// The command "LOGON FAILED"
        /// </summary>
        public override string Command { get; } = "LOGON FAILED";

        /// <summary>
        /// The process of the packet
        /// </summary>
        /// <param name="packet"></param>
        public override void Process(Packet packet)
        {
            var reason = packet["REASON"];
            if (reason == "32")
            {
                Client.DefaultHost = packet.Payload;
                Bot.Redirect = false;
                Bot.Login(Bot.Settings);
                return;
            }
            Bot.On.Trigger("lf", reason);
        }
    }
}
