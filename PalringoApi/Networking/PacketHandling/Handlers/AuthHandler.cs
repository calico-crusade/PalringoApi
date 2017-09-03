using PalringoApi.Utilities;

namespace PalringoApi.Networking.PacketHandling.Handlers
{
    /// <summary>
    /// Handles when an Authorization packet is obtained
    /// </summary>
    public class AuthHandler : PacketHandler
    {
        /// <summary>
        /// The command "AUTH"
        /// </summary>
        public override string Command { get; } = "AUTH";

        /// <summary>
        /// What happens when the packet is processed
        /// </summary>
        /// <param name="packet"></param>
        public override void Process(Packet packet)
        {
            byte[] en = Auth.generateAuth(Static.PalringoEncoding.GetBytes(packet.Payload), Static.PalringoEncoding.GetBytes(Bot.Settings.Password));
            Client.WritePacket(PacketTemplates.Auth(en, Bot.Settings.Visibility));
        }
    }
}
