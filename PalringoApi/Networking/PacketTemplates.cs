using PalringoApi.Subprofile.Parsing;
using PalringoApi.Subprofile.Types;
using System;
using System.Collections.Generic;

namespace PalringoApi.Networking
{
    /// <summary>
    /// Different packet templates
    /// Not going to bother commenting them all... They're pretty self explanitory
    /// </summary>
    public static class PacketTemplates
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="device"></param>
        /// <param name="clientVersion"></param>
        /// <param name="redirect"></param>
        /// <param name="spamfilter"></param>
        /// <returns></returns>
        public static Packet Login(string email, string device, string clientVersion = "", bool redirect = false, bool spamfilter = false)
        {
            int mflags = 786437;
            //mflags |= 0x4;
            //mflags |= 0x80000;
            if (spamfilter)
                mflags |= 0x1000;
            //mflags |= 0x100;
            var p = new Packet
            {
                Command = "LOGON",
                Headers = new Dictionary<string, string>
                {
                    ["app-type"] = device,
                    ["capabilities"] = mflags.ToString(),
                    ["client-version"] = "2.8.1, 60842",
                    ["fw"] = "Win 6.2",
                    ["last"] = "1",
                    ["protocol-version"] = "2.0",
                    ["name"] = email,
                    ["redirect-count"] = redirect ? "1" : "0"
                }
            };
            return p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Packet Auth(byte[] password, AuthStatus type)
        {
            return new Packet
            {
                Command = "AUTH",
                Headers = new Dictionary<string, string>
                {
                    ["Encryption-Type"] = "1",
                    ["Online-Status"] = ((int)type).ToString(),
                    ["Last"] = "1"
                },
                Payload = Static.PalringoEncoding.GetString(password)
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Packet Ping()
        {
            return new Packet() { Command = "P" };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static Packet JoinGroup(string GroupID, string Password = null)
        {
            Packet pcck = new Packet();
            pcck.Command = "GROUP SUBSCRIBE";
            pcck.Headers.Add("Name", GroupID);
            if (!string.IsNullOrEmpty(Password))
            {
                pcck.Payload = Password;
            }
            return pcck;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public static Packet LeaveGroup(int GroupID)
        {
            Packet pcck = new Packet();
            pcck.Command = "GROUP UNSUB";
            pcck.Headers.Add("group-id", GroupID.ToString());
            return pcck;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="Description"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static Packet CreateGroup(string GroupID, string Description, string Password = null)
        {
            Packet pcck = new Packet();
            pcck.Command = "GROUP CREATE";
            pcck.Headers.Add("Name", GroupID);
            pcck.Headers.Add("Desc", Description);
            if (!String.IsNullOrEmpty(Password))
            {
                pcck.Payload = Password;
            }
            return pcck;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Packet DeleteContact(int ID)
        {
            Packet pck = new Packet();
            pck.Command = "CONTACT UPDATE";
            pck.Headers.Add("contact-id", ID.ToString());
            pck.Headers.Add("remove", true.ToString());
            return pck;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="groupId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Packet AdminAction(int action, int groupId, int userId)
        {
            var p = new Packet();
            p.Command = "GROUP ADMIN";
            p.Headers.Add("group-id", groupId.ToString());
            p.Headers.Add("target-id", userId.ToString());
            p.Headers.Add("last", "1");
            p.Headers.Add("action", action.ToString());
            return p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Action"></param>
        /// <param name="RoomID"></param>
        /// <param name="targetID"></param>
        /// <returns></returns>
        public static Packet AdminAction(AdminActions Action, int RoomID, int targetID)
        {
            Packet p = new Packet();
            p.Command = "GROUP ADMIN";
            p.Headers.Add("group-id", RoomID.ToString());
            p.Headers.Add("target-id", targetID.ToString());
            p.Headers.Add("last", "1");
            p.Headers.Add("action", ((int)Action).ToString());
            return p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="USER"></param>
        /// <returns></returns>
        public static Packet AddContact(int USER)
        {
            Packet p = new Packet();
            p.Command = "CONTACT ADD";
            p.Headers.Add("last", "1");
            p.Headers.Add("target-id", USER.ToString());
            p.Payload = "I'd like to add you";
            return p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static Packet ContactResponse(bool accept, int userid)
        {
            return new Packet()
            {
                Command = "CONTACT ADD RESP",
                Headers = new Dictionary<string, string>
                {
                    {"accepted", accept ? "1" : "0"},
                    {"last", "1"},
                    {"source-id", userid.ToString()}
                }
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static Packet UpdateInfo(string nickname, string status)
        {
            Packet p = new Packet();
            p.Command = "CONTACT DETAIL";
            p.Headers.Add("last", "1");
            p.Headers.Add("nickname", nickname);
            p.Headers.Add("status", status);
            return p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static Packet HTMLTest(int room, string HTML)
        {
            Packet p = new Packet();
            p.Command = "MESG";
            p.Headers.Add("content-type", "image/jpeghtml");
            p.Headers.Add("last", "T");
            p.Headers.Add("content-length", HTML.Length.ToString());
            p.Headers.Add("target-id", room.ToString());
            p.Payload = HTML;
            return p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Packet Profile(string ID)
        {
            Packet packet = new Packet();
            packet.Command = "SUB PROFILE QUERY";
            var datamap = new DataMap();
            datamap.SetValue("Sub-Id", ID);
            packet.Payload = Static.PalringoEncoding.GetString(datamap.Serialize());
            return packet;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="resultcount"></param>
        /// <returns></returns>
        public static Packet GroupQuery(string name, int resultcount)
        {
            var map = new DataMap();
            var pack = new Packet { Command = "PROVIF QUERY" };
            map.SetValue("action", "palringo_groups");
            map.SetValue("parameters", "offset=0&max_results=" + resultcount + "&name=" + System.Uri.EscapeDataString(name));
            pack.Payload = Static.PalringoEncoding.GetString(map.Serialize());
            return pack;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public static Packet GroupQuery(int GroupID)
        {
            var newmap = new DataMap();
            var pcck = new Packet { Command = "PROVIF QUERY" };
            newmap.SetValue("action", "palringo_group_info");
            newmap.SetValue("parameters", "id=" + GroupID);
            pcck.Payload = Static.PalringoEncoding.GetString(newmap.Serialize());
            return pcck;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static Packet UserQuery(int userid)
        {
            var newmap = new DataMap();
            var pcck = new Packet { Command = "PROVIF QUERY" };
            newmap.SetValue("action", "palringo_profile_info");
            newmap.SetValue("parameters", "id=" + userid);
            pcck.Payload = Static.PalringoEncoding.GetString(newmap.Serialize());
            return pcck;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="group"></param>
        /// <param name="lastmessagetimestamp"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Packet MessageHistory(int id, bool group, string lastmessagetimestamp, int count = 3)
        {
            return new Packet()
            {
                Command = "MESG HIST",
                Headers = new Dictionary<string, string>
                {
                    {"count", "-" + count},
                    {group ? "from-group" : "from-private", lastmessagetimestamp},
                    {"last", "1"},
                    {"source-id", id.ToString()}
                },
                Payload = ""
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastonline"></param>
        /// <returns></returns>
        public static Packet GetStoredMessages(string lastonline)
        {
            return new Packet()
            {
                Command = "MESG STORED",
                Headers = new Dictionary<string, string>
                {
                    {"count", "0"},
                    {"from-private", lastonline},
                    {"include-self", "0"},
                    {"last", "1"}
                },
                Payload = ""
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgpackId"></param>
        /// <param name="content"></param>
        /// <param name="id"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static Packet MessagePack(int msgpackId, string content, int id, bool group)
        {
            string msg = System.Uri.EscapeDataString(content);
            var map = new DataMap();
            map.SetValue("parameters",
                string.Format("format=json&action=palringo.message.pack.entry.send&message_entry_id={0}" +
                "&message_content={1}&target_id={2}&is_group={3}", msgpackId, msg, id, group ? "1" : "0"));
            map.SetValue("url", "api");
            return new Packet()
            {
                Command = "URL QUERY",
                Headers = new Dictionary<string, string>
                {
                    {"last", "1"}
                },
                Payload = Static.PalringoEncoding.GetString(map.Serialize())
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Packet UrlQuery(string parameters, string url)
        {
            var dm = new DataMap();
            dm.SetValue("parameters", parameters);
            dm.SetValue("url", url);
            return new Packet
            {
                Command = "URL QUERY",
                Headers = new Dictionary<string, string>
                {
                    {"last", "1"}
                },
                Payload = Static.PalringoEncoding.GetString(dm.Serialize())
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Packet Bye()
        {
            return new Packet { Command = "BYE" };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="contentType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Packet Command(string action, string contentType, string data)
        {
            return new Packet
            {
                Command = "COMMAND",
                Headers = new Dictionary<string, string>
                {
                    ["action"] = action,
                    ["content-type"] = contentType,
                    ["version"] = "2",
                    ["last"] = "T"
                },
                Payload = data
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public static Packet SpamFilter(bool enabled)
        {
            return Command("set_spam_filter", "application/json", $"{{\"spamFilterEnabled\": {enabled.ToString()}}}");
        }

        /// <summary>
        /// Creates a packet for updating the current users profile (Extended only)
        /// </summary>
        /// <param name="profile">The users profile</param>
        /// <returns>The packet to send.</returns>
        public static Packet SubProfileUpdate(Subprofile.ExtendedUser profile)
        {
            var map = profile.ToMap();
            var extMap = new DataMap();
            extMap.SetValueRaw("ext", map.Serialize());
            var dataMap = new DataMap();
            dataMap.SetValueRaw("user_data", extMap.Serialize());

            return new Packet
            {
                Command = "SUB PROFILE UPDATE",
                Headers = new Dictionary<string, string>(),
                Payload = Static.PalringoEncoding.GetString(dataMap.Serialize())
            };
        }
    }
}
