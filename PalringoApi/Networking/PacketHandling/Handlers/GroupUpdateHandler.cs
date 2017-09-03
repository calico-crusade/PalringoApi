using PalringoApi.PacketData;
using PalringoApi.Subprofile;
using PalringoApi.Subprofile.Parsing;

namespace PalringoApi.Networking.PacketHandling.Handlers
{
    /// <summary>
    /// Group Update packet handling
    /// </summary>
    public class GroupUpdateHandler : PacketHandler
    {
        /// <summary>
        /// The command "GROUP UPDATE"
        /// </summary>
        public override string Command => "GROUP UPDATE";

        /// <summary>
        /// The packet process
        /// </summary>
        /// <param name="packet"></param>
        public override void Process(Packet packet)
        {
            var d = DataMap.Deserialize(packet.Payload);

            UserId uid = d["contact-id"];
            GroupId gid = d["group-id"];
            int type = int.Parse(d["type"]);

            var user = uid.GetUser(Bot);

            if (user == null)
            {
                user = new User(uid);
                Bot.Information.UserCache.Add(uid, user);
            }

            if (type == 0)
            {
                if (d.ContainsKey("contacts"))
                    d = DataMap.Deserialize(d["contacts"]);
                if (d.ContainsKey(uid.ToString()))
                    d = DataMap.Deserialize(d[uid.ToString()]);
                if (d.ContainsKey("nickname"))
                    user.Nickname = d["nickname"].GetDisplayString();
                if (d.ContainsKey("status"))
                    user.Status = d["status"].GetDisplayString();
                if (d.ContainsKey("rep"))
                    user.Reputation = int.Parse(d["rep"]);
                if (d.ContainsKey("rep_lvl"))
                    user.RepLevel = double.Parse(d["rep_lvl"]);
            }

            var g = new GroupUpdate
            {
                Group = gid,
                User = uid,
                IsJoin = type == 0
            };
            Bot.On.Trigger("gu", g);
        }
    }
}
