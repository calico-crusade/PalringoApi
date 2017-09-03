using System;
using PalringoApi.PacketData;
using rs = System.Action<PalringoApi.PacketData.Response>;
using PalringoApi.Subprofile;
using PalringoApi.Subprofile.Parsing;

namespace PalringoApi.Networking
{
    public partial class Packet
    {
        /// <summary>
        /// Watch a packet for a certain packet matching parameters that relate to this one
        /// </summary>
        /// <typeparam name="T">The object to map the packet too</typeparam>
        /// <param name="bot">The instance of the PalBot to attach to</param>
        /// <param name="msg">The packet command to watch for</param>
        /// <param name="resp">The action to call on the response</param>
        /// <param name="pt">The Packet Transformer from the <see cref="Packet"/> to the <typeparamref name="T"/></param>
        /// <param name="m">The function that determinds if the packet is a match</param>
        /// <returns>Returns this instance of the packet (string-able-ness)</returns>
        public Packet Watch<T>(PalBot bot, string msg, Action<T> resp, Func<Packet, T> pt, Func<T, bool> m)
        {
            bot.Callbacks.Watch(msg, resp, pt, m);
            return this;
        }

        /// <summary>
        /// Watches a packet for a RESPONSE packet
        /// </summary>
        /// <param name="bot">The bot instance to attach to</param>
        /// <param name="resp">The action to do upon obtaining the response</param>
        /// <returns>This instance of the packet (for string-able-ness)</returns>
        public Packet Watch(PalBot bot, rs resp)
        {
            if (resp == null)
                return this;

            //bot.Callbacks.Watch("RESPONSE", MessageId, resp, (p) => p.Map<Response>());
            Watch(bot, "RESPONSE", resp, (p) => p.Map<Response>(), (r) => r.MessageId == MessageId);
            return this;
        }

        /// <summary>
        /// Watches for a response to the a user profile request
        /// </summary>
        /// <param name="Bot">The bot instance to attach to</param>
        /// <param name="uid">The user id to look for</param>
        /// <param name="resp">The action to do upon finishing</param>
        /// <returns>This instance of the packet (for string-able-ness)</returns>
        public Packet Watch(PalBot Bot, UserId uid, Action<User> resp)
        {

            if (resp == null)
                return this;

            Bot.Callbacks.Watch("SUB PROFILE QUERY RESULT", resp, (packet) =>
            {
                var bytes = Static.PalringoEncoding.GetBytes(packet.Payload);
                var map = new DataMap(bytes);
                
                if (map.ContainsKey("sub-id"))
                {
                    int id = map.GetValueInt("sub-id");

                    if (!Bot.Information.UserCache.ContainsKey(id))
                        Bot.Information.UserCache.Add(id, new User());
                    
                    foreach (var m in map.EnumerateMaps())
                    {
                        Bot.Information.UserCache[id].Parse(Bot, m);
                    }

                    return Bot.Information.UserCache[id];
                }
                return null;
            }, (t) => t.Id == uid);
            return this;
        }
    }
}
