using PalringoApi.Subprofile.Types;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Serialization;

namespace PalringoApi.Utilities
{
    /// <summary>
    /// Login and bot settings
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Registry key for saving data
        /// </summary>
        public static string RegistrySubKeyBase { get; set; } = "SOFTWARE\\";
        
        /// <summary>
        /// Email address for the bot
        /// </summary>
        [XmlAttribute]
        public string Email { get; set; }

        /// <summary>
        /// password for the bot
        /// </summary>
        [XmlAttribute]
        public string Password { get; set; }

        /// <summary>
        /// What the visibility of the bot should be
        /// </summary>
        [XmlAttribute]
        public AuthStatus Visibility { get; set; }

        /// <summary>
        /// What device type to login as
        /// </summary>
        [XmlAttribute]
        public DeviceType Device { get; set; }

        /// <summary>
        /// Whether or not to turn on spam filter
        /// </summary>
        [XmlAttribute]
        public bool SpamFilter { get; set; }

        /// <summary>
        /// Whether or not to Track Messages
        /// Not implemented yet.
        /// </summary>
        [XmlAttribute]
        public bool TrackMessages { get; set; }

        /// <summary>
        /// All of the administrators palringo ids
        /// </summary>
        [XmlAttribute]
        public int[] Admins { get; set; } 

        /// <summary>
        /// Saves settings to a file for later
        /// </summary>
        /// <param name="filename"></param>
        public void SaveToFile(string filename)
        {
            File.WriteAllText(filename, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        /// <summary>
        /// saves settings to registry for later
        /// </summary>
        /// <param name="appname"></param>
        public void SaveToRegistry(string appname)
        {
            Reg.SubKey = RegistrySubKeyBase + appname;
            Reg.Write("settings", JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Settings() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="vis"></param>
        /// <param name="device"></param>
        /// <param name="spamfilter"></param>
        /// <param name="trackmessages"></param>
        public Settings(string email, string password, AuthStatus vis, DeviceType device, bool spamfilter, bool trackmessages)
        {
            Email = email;
            Password = password;
            Visibility = vis;
            Device = device;
            SpamFilter = spamfilter;
            TrackMessages = trackmessages;
        }

        /// <summary>
        /// Loads from JSON file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="provideDefaults"></param>
        /// <returns></returns>
        public static Settings FromFile(string filename, bool provideDefaults = true)
        {
            if (!File.Exists(filename))
                return provideDefaults ? Default : null;

            try
            {
                return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(filename));
            }
            catch
            {
                if (provideDefaults)
                    return Default;
                throw new System.Exception("Failed to deserialize settings file.");
            }
        }

        /// <summary>
        /// Loads from registry
        /// </summary>
        /// <param name="appname"></param>
        /// <param name="provideDefaults"></param>
        /// <returns></returns>
        public static Settings FromRegistry(string appname, bool provideDefaults = true)
        {
            Reg.SubKey = RegistrySubKeyBase + appname;

            try
            {
                var data = Reg.Read("settings");
                if (data == null)
                    return provideDefaults ? Default : null;
                return JsonConvert.DeserializeObject<Settings>(Reg.Read("settings"));
            }
            catch
            {
                if (provideDefaults)
                    return Default;
                throw new System.Exception("Failed to deserialize settings from registry.");
            }
        }
        
        /// <summary>
        /// Defaults 
        /// </summary>
        public static Settings Default => new Settings
        {
            Email = "",
            Password = "",
            Visibility = AuthStatus.Online,
            Device = DeviceType.PC,
            SpamFilter = false,
            TrackMessages = true
        };
    }
}
