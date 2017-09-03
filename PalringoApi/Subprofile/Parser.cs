using PalringoApi.Networking;
using PalringoApi.Subprofile.Parsing;
using System;
using System.Collections.Generic;

namespace PalringoApi.Subprofile
{
    /// <summary>
    /// Parses sub profile to provide group and user information
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Collection of group profiles
        /// </summary>
        public Dictionary<GroupId, Group> Groups { get; set; } = new Dictionary<GroupId, Group>();
        /// <summary>
        /// Collection of user profiles
        /// </summary>
        public Dictionary<UserId, User> UserCache { get; set; } = new Dictionary<UserId, User>();
        /// <summary>
        /// Collection of the bots contacts
        /// </summary>
        public Dictionary<UserId, User> Contacts { get; set; } = new Dictionary<UserId, User>();
        /// <summary>
        /// The current accounts profile
        /// </summary>
        public ExtendedUser Profile { get; set; } = new ExtendedUser();

        /// <summary>
        /// Default constructor
        /// </summary>
        public Parser() { }

        /// <summary>
        /// Parses sub profile information given from the packet
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="packet"></param>
        public void Parse(PalBot bot, Packet packet)
        {
            DataMap map;
            if (packet["IV"] != null && packet["RK"] != null)
            {
                int iv = int.Parse(packet["IV"]);
                int rk = iv != 0 ? int.Parse(packet["RK"]) : 0;
                map = new DataMap();
                map.Deserialize(Static.PalringoEncoding.GetBytes(packet.Payload), iv, packet.Payload.Length - iv - rk);
            }
            else
                map = new DataMap(Static.PalringoEncoding.GetBytes(packet.Payload));

            try
            {
                var contacts = map.GetValueMapAll("contacts");
                if (contacts != null)
                {
                    foreach(var item in contacts.Data)
                    {
                        UserId id;
                        if (!UserId.TryParse(item.Key, out id))
                            continue;

                        var cm = new DataMap(item.Value);
                        if (UserCache.ContainsKey(id))
                            UserCache[id].Parse(bot, cm);
                        else
                        {
                            var u = new User(id);
                            u.Parse(bot, cm);
                            UserCache.Add(id, u);
                        }
                    }
                }

                var groups = map.GetValueMap("group_sub");
                if (groups != null)
                {
                    foreach(var gm in groups.Data)
                    {
                        int id;
                        if (!int.TryParse(gm.Key, out id))
                            continue;
                        if (Groups.ContainsKey(id))
                            Groups[id].Parse(bot, new DataMap(gm.Value));
                        else
                        {
                            var gp = new Group { Id = id };
                            gp.Parse(bot, new DataMap(gm.Value));
                            Groups.Add(id, gp);
                            bot.On.Trigger("gj", gp);
                        }
                    }
                }

                var ext = map.GetValueMap("ext");
                if (ext != null)
                    Profile.Parse(bot, ext);

                if (map != null)
                    Profile.Parse(bot, map);
            }
            catch (Exception ex)
            {
                bot.On.Trigger("e", ex);
            }
        }
    }
}
