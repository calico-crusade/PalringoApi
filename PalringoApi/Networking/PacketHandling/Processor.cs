using System;
using System.Collections.Generic;
using System.Linq;

namespace PalringoApi.Networking.PacketHandling
{
    /// <summary>
    /// The packet processor
    /// </summary>
    public class Processor
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Processor()
        {
        }

        private Dictionary<string, Type> PacketHandlers { get; set; }

        private Dictionary<long, List<Packet>> ConjoinedPackets { get; set; } = new Dictionary<long, List<Packet>>();

        private void LoadPacketHandlers(PalBot bot)
        {
            try
            {
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(t => t.GetTypes())
                    .Where(typeof(IPacketHandler).IsAssignableFrom)
                    .Where(t => !t.IsInterface && !t.IsAbstract)
                    .ToArray();

                PacketHandlers = new Dictionary<string, Type>();
                foreach (var type in types)
                {
                    var i = (IPacketHandler)Activator.CreateInstance(type);
                    if (!PacketHandlers.ContainsKey(i.Command.ToUpper()))
                        PacketHandlers.Add(i.Command.ToUpper(), type);
                }
            }
            catch (Exception ex)
            {
                bot.On.Trigger("e", ex);
            }
        }

        /// <summary>
        /// The processing system
        /// </summary>
        /// <param name="bot">The bot to attach to</param>
        /// <param name="inPacket">The packet coming in</param>
        /// <param name="client">The client to attach to</param>
        public void Process(PalBot bot, Packet inPacket, Client client)
        {
            Packet packet;
            if (!CheckPacket(inPacket, out packet))
                return;

            if (PacketHandlers == null || PacketHandlers.Count == 0)
                LoadPacketHandlers(bot);

            if (!PacketHandlers.ContainsKey(packet.Command.ToUpper()))
            {
                bot.On.Trigger("pk", packet);
                return;
            }

            if (packet["COMPRESSION"] == "1")
            {
                packet.Payload = packet.DecompressPayload();
            }

            try
            {
                bot.Callbacks.Activate(packet);

                var plug = PacketHandlers[packet.Command.ToUpper()];
                var i = (IPacketHandler)Activator.CreateInstance(plug);

                i.Bot = bot;
                i.Client = client;
                i.Process(packet);
            }
            catch (Exception ex)
            {
                bot.On.Trigger("e", ex);
            }
        }

        private bool CheckPacket(Packet packet, out Packet modPacket)
        {
            if (packet.Headers.ContainsKey("TOTAL-LENGTH") && packet["TOTAL-LENGTH"] != packet.ContentLength.ToString())
            {
                var mid = long.Parse(packet["MESG-ID"]);
                ConjoinedPackets.Add(mid, new List<Packet> { packet });
                modPacket = null;
                return false;
            }
            else if (packet.Headers.ContainsKey("CORRELATION-ID"))
            {
                var mid = long.Parse(packet["CORRELATION-ID"]);
                if (ConjoinedPackets.ContainsKey(mid))
                    ConjoinedPackets[mid].Add(packet);
                else
                    ConjoinedPackets.Add(mid, new List<Packet> { packet });

                if (packet["LAST"] == "T" || packet["LAST"] == "1")
                {
                    var packets = ConjoinedPackets[mid];
                    modPacket = new Packet
                    {
                        Command = packets[0].Command,
                        Headers = new Dictionary<string, string>(),
                        Payload = string.Join("", packets.Select(t => t.Payload).ToArray())
                    };

                    foreach (var p in packets)
                    {
                        foreach(var h in p.Headers)
                        {
                            if (!modPacket.Headers.ContainsKey(h.Key))
                                modPacket[h.Key] = h.Value;
                        }
                    }
                    
                    ConjoinedPackets.Remove(mid);
                    return true;
                }

                modPacket = null;
                return false;
            }

            modPacket = packet;
            return true;
        }
    }
}
