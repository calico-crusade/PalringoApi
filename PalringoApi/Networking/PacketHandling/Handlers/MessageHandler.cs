using PalringoApi.PacketData;
using PalringoApi.Subprofile.Types;
using System.Collections.Generic;

namespace PalringoApi.Networking.PacketHandling.Handlers
{
    /// <summary>
    /// Handles Message packets
    /// </summary>
    public class MessageHandler : PacketHandler
    {
        /// <summary>
        /// The command "MESG"
        /// </summary>
        public override string Command { get; } = "MESG";

        private Dictionary<long, Packet> InitialVms = new Dictionary<long, Packet>();

        /// <summary>
        /// The packet process
        /// </summary>
        /// <param name="packet"></param>
        public override void Process(Packet packet)
        {
            var msg = Map<Message>(packet);

            if (msg.ContentType == DataType.VoiceMessage)
            {
                if (packet.Headers.ContainsKey("LAST") && !packet.Headers.ContainsKey("CORRELATION-ID"))
                {
                    DoTrigger(msg);
                    return;
                }

                if (packet.Headers.ContainsKey("CORRELATION-ID"))
                {
                    var corid = long.Parse(packet.Headers["CORRELATION-ID"]);
                    if (InitialVms.ContainsKey(corid))
                    {
                        InitialVms[corid].Payload += packet.Payload;
                        if (packet.Headers.ContainsKey("LAST"))
                        {
                            var pk = Map<Message>(InitialVms[corid]);
                            InitialVms.Remove(corid);
                            DoTrigger(pk);
                            return;
                        }
                        return;
                    }
                    InitialVms.Add(corid, packet);
                    return;
                }

                InitialVms.Add(packet.MessageId, packet);
                return;
            }

            msg.Content = msg.Content.GetDisplayString();
            DoTrigger(msg);
        }

        private void DoTrigger(Message msg)
        {
            if (msg.MesgType == MessageType.Group)
                Bot.On.Trigger("gm", msg);
            else
                Bot.On.Trigger("pm", msg);
        }
    }
}
