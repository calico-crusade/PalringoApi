using System;

namespace PalringoApi.Networking.PacketHandling.Handlers
{
    /// <summary>
    /// The throttle handler
    /// </summary>
    public class ThrottleHandler : PacketHandler
    {
        /// <summary>
        /// The command
        /// </summary>
        public override string Command => "THROTTLE";

        /// <summary>
        /// The processor
        /// </summary>
        /// <param name="packet"></param>
        public override void Process(Packet packet)
        {
            try
            {
                var comman = packet["COMMAND"];
                var durati = packet["DURATION"];
                var reason = packet["REASON"];
                Bot.On.Trigger("th", int.Parse(durati));
            }
            catch (Exception ex)
            {
                Bot.On.Trigger("e", ex);
            }
        }
    }
}
