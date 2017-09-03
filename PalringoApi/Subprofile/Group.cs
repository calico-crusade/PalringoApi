using PalringoApi.Subprofile.Types;
using System.Collections.Generic;
using Newtonsoft.Json;
using PalringoApi.Subprofile.Parsing;
using PalringoApi.Networking;

namespace PalringoApi.Subprofile
{
    /// <summary>
    /// Groups profile (most properties are self explainitory so they are uncommented)
    /// </summary>
    public class Group : IParsable
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("long_description")]
        public string LongDescription { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public UserId OwnerId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("id")]
        public GroupId Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("entry_level")]
        public int EntryLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("premium")]
        public bool Premium { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool YouOwn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("locked")]
        public bool Locked { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("pswd")]
        public bool PasswordRequired { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("advanced_admin")]
        public bool FullAdmin { get; set; }
        /// <summary>
        /// They can't be seen from search list
        /// </summary>
        [JsonProperty("discoverability")]
        public bool ExFilter { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("lon")]
        public float Latitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("lat")]
        public float Longitude { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("language")]
        public Language Language { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("catagories")]
        public List<GroupCategory> Categories { get; set; } = new List<GroupCategory>();
        /// <summary>
        /// No clue, tbh?
        /// </summary>
        public List<GroupOptions> Discoverability { get; set; } = new List<GroupOptions>();
        /// <summary>
        /// No clue tbh?
        /// </summary>
        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = new List<string>();
        /// <summary>
        /// Whether or not the group exists (if its not null but has no data)
        /// </summary>
        public bool Exists { get; set; } = true;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Group() { }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="exists"></param>
        public Group(GroupId id, bool exists)
        {
            Id = id;
            Exists = exists;
        }

        /// <summary>
        /// Collection of the groups members and their roles
        /// </summary>
        public Dictionary<UserId, GroupUser> Members { get; set; } = new Dictionary<UserId, GroupUser>();

        /// <summary>
        /// You can subscribe here to know when the group is updated
        /// </summary>
        public event DataCarrier Updated = delegate { };

        /// <summary>
        /// Parse the data from the DataMap given by subprofile
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="data"></param>
        public void Parse(PalBot bot, DataMap data)
        {
            foreach (var item in data.StrData)
            {
                if (item.Key == "desc")
                    Description = item.Value.GetDisplayString();
                else if (item.Key == "name")
                    Name = item.Value.GetDisplayString();
                else if (item.Key == "owner")
                    OwnerId = item.Value;
                else if (item.Key == "attributes")
                {
                    var t2 = DataMap.Deserialize(item.Value);
                    foreach (var it in t2)
                    {
                        var t3 = DataMap.Deserialize(it.Value);
                        string val = t3["data"];
                        int atri = int.Parse(t3["attribute_type"]);
                        #region Attributes
                        switch (atri)
                        {
                            case 1:
                                LongDescription = val.GetDisplayString();
                                break;
                            case 2: //Locked
                                Locked = false;
                                break;
                            case 4: //CONSENT - Full Admin? 
                                break;
                            case 5: //Language ID
                                Language = (Language)int.Parse(val);
                                break;
                            case 6: //PAYTIER (Proxy Admin?)

                                break;
                            case 7: //PAYPERIOD (Proxy Admin?)

                                break;
                            case 8: //AVATAR

                                break;
                            case 9: //Longitude
                                Longitude = float.Parse(val);
                                break;
                            case 10: //Latitude
                                Latitude = float.Parse(val);
                                break;
                            case 11:
                                Premium = true;
                                break;
                            case 12: //Permanent (Proxy Admin?)

                                break;
                            case 14: //Weight

                                break;
                            case 15: //Tags
                            case 16:
                            case 17:
                            case 18:
                            case 19:
                                Tags.Add(val.GetDisplayString());
                                break;
                            case 20: //Catagories
                            case 21:
                            case 22:
                            case 23:
                            case 24:
                                Categories.Add((GroupCategory)int.Parse(val));
                                break;
                            case 25: //Entry Level
                                EntryLevel = int.Parse(val);
                                break;
                            case 28:

                                break;
                            case 13://Discoverability shit
                            case 3:
                            case 32:
                                Discoverability.Add((GroupOptions)int.Parse(val));
                                break;
                            case 26://Anonymous Listeners (Proxy Admin?)

                                break;
                        }
                        #endregion
                    }
                }
                else
                {
                    UserId uid;
                    if (!UserId.TryParse(item.Key, out uid))
                        continue;

                    var gm = Members.ContainsKey(uid) ? Members[uid] : GroupUser.FromUser(uid.GetUser(bot) ?? new User(uid));

                    var t1 = DataMap.Deserialize(item.Value);
                    if (t1.ContainsKey("capabilities"))
                        gm.UserRole = OwnerId == uid ? Role.Owner : (Role)int.Parse(t1["capabilities"]);

                    if (!Members.ContainsKey(uid))
                        Members.Add(uid, gm);
                }
            }

            if (Members.ContainsKey(OwnerId))
                Members[OwnerId].UserRole = Role.Owner;

            Updated();
        }
    }
}
