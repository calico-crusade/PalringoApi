using PalringoApi.Networking;
using PalringoApi.Subprofile.Parsing;
using PalringoApi.Subprofile.Types;
using System.Collections.Generic;

namespace PalringoApi.Subprofile
{
    /// <summary>
    /// Users profile, most of it is self explainitory, not going to comment
    /// </summary>
    public class User : IParsable
    {
        /// <summary>
        /// 
        /// </summary>
        public UserId Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Nickname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// This contains what their <see cref="Tags"/>
        /// </summary>
        public uint Privileges { get; set; }
        /// <summary>
        /// This is an older version of Rep Level
        /// </summary>
        public long Reputation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double RepLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Device DeviceType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Status OnlineStatus { get; set; }
        /// <summary>
        /// Added for Brasil
        /// </summary>
        public UserRole TagRole
        {
            get
            {
                if (Staff)
                    return UserRole.Staff;
                if (Vip)
                    return UserRole.Vip;
                if (Agent)
                    return UserRole.Agent;
                if (AlphaTester)
                    return UserRole.AlphaTester;
                if (Pest)
                    return UserRole.Pest;
                return UserRole.User;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        public User(UserId id)
        {
            Id = id;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public User() { }

        /// <summary>
        /// All of the tags the user has
        /// </summary>
        public List<Tags> PrivTags
        {
            get
            {
                var tag = new List<Tags>();
                if (Premium) tag.Add(Tags.Premium);
                if (Staff) tag.Add(Tags.Staff);
                if (Agent) tag.Add(Tags.Agent);
                if (SuperAdmin) tag.Add(Tags.SuperAdmin);
                if (Vip) tag.Add(Tags.Vip);
                if (ValidEmail) tag.Add(Tags.ValidEmail);
                if (AlphaTester) tag.Add(Tags.AlphaTester);
                if (Pest) tag.Add(Tags.Pest);
                return tag;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Premium
        {
            get { return (Privileges & 0x100000) != 0; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Staff
        {
            get { return (Privileges & 0x1000) != 0; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Agent
        {
            get { return (Privileges & 0x10000000) != 0; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool SuperAdmin
        {
            get { return (Privileges & 0x10000) != 0; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Vip
        {
            get { return (Privileges & 0x200000) != 0; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool ValidEmail
        {
            get { return (Privileges & 0x80000) != 0; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool AlphaTester
        {
            get { return (Privileges & 0x800) != 0; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool Pest
        {
            get { return (Privileges & 0x40000) != 0; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool HasTag
        {
            get { return Staff || Agent || SuperAdmin || Vip || AlphaTester; }
        }

        /// <summary>
        /// You can subscribe to this to get when the user profile updates
        /// </summary>
        public event DataCarrier Updated = delegate { };

        /// <summary>
        /// Parse the users profile from subprofile
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="data"></param>
        public void Parse(PalBot bot, DataMap data)
        {
            if (data.ContainsKey("sub-id"))
                Id = data.GetValueInt("sub-id");
            if (data.ContainsKey("status"))
                Status = data.GetValue("status");
            if (data.ContainsKey("rep_lvl"))
                RepLevel = double.Parse(data.GetValue("rep_lvl"));
            if (data.ContainsKey("nickname"))
                Nickname = data.GetValue("nickname");
            if (data.ContainsKey("rep"))
                Reputation = long.Parse(data.GetValue("rep"));
            if (data.ContainsKey("privileges"))
                Privileges = uint.Parse(data.GetValue("privileges"));
            if (data.ContainsKey("online-status"))
                OnlineStatus = (Status)int.Parse(data.GetValue("online-status"));
            if (data.ContainsKey("device-type"))
                DeviceType = (Device)int.Parse(data.GetValue("device-type"));

            if (data.ContainsKey("contact") && data.GetValue("contact") == "1" && !bot.Information.Contacts.ContainsKey(Id))
                bot.Information.Contacts.Add(Id, this);

            Updated();
        }

        /// <summary>
        /// Converts it to an easy to read JSON string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
