using System;
using PalringoApi.Networking;
using PalringoApi.Networking.PacketHandling;
using PalringoApi.PluginManagement;
using PalringoApi.Subprofile;
using PalringoApi.Subprofile.Types;
using PalringoApi.Utilities;
using System.Linq;

namespace PalringoApi
{
    /// <summary>
    /// The entry point for the palringo api
    /// </summary>
    public partial class PalBot
    {
        private Client _client;
        private Processor _processor;
        private bool _loggedIn = false, 
            _registering = false, 
            _loadPlugins = true;

        /// <summary>
        /// The title for the bot
        /// Use only if you are using mutliple bots in 1 application
        /// </summary>
        public string Title { get; set; } = null;
        /// <summary>
        /// Whether or not to redirect on a LoginFailed packet containing an IP address (load blancer)
        /// </summary>
        public bool Redirect { get; set; } = true;
        /// <summary>
        /// Log packets that are sent through
        /// </summary>
        public bool PacketLog { get; set; } = false;
        /// <summary>
        /// Watches for the throttle packet and delays packet as not to lose messages
        /// </summary>
        public bool WatchForThrottle { get; set; } = true;
        /// <summary>
        /// The login settings for the bot
        /// </summary>
        public Settings Settings { get; set; }
        /// <summary>
        /// The event handlers for the bot
        /// </summary>
        public Events On { get; private set; }
        /// <summary>
        /// The information cache for the bot
        /// </summary>
        public Parser Information { get; private set; }
        /// <summary>
        /// The manager of all the plugins loaded into the bot
        /// </summary>
        public PluginManager Plugins { get; private set; }
        /// <summary>
        /// Any methods that are required for Callbacks (mostly messages and information)
        /// </summary>
        public CallBacks Callbacks { get; private set; }
        /// <summary>
        /// Queue for messages that aids in stopping throttle from occuring
        /// </summary>
        public MessageQueue MessageQueue { get; private set; }
        /// <summary>
        /// Used to check whether the bot is logged in or not
        /// </summary>
        public bool LoggedIn => _loggedIn;
        
        /// <summary>
        /// Constructor for the bot
        /// </summary>
        public PalBot()
        {
            _processor = new Processor();
            _client = new Client();
            Information = new Parser();
            Callbacks = new CallBacks();
            On = new Events();
            On.LoginSuccess += () => _loggedIn = true;
            On.LoginFailed += (r) => _loggedIn = false;
            On.GroupLeft += (g) =>
            {
                if (Information.Groups.ContainsKey(g.Id))
                    Information.Groups.Remove(g.Id);
            };
            On.GroupMessage += MessageRecieved;
            On.PrivateMessage += MessageRecieved;
            _client.OnPacketReceived += PacketRecieved;
            _client.OnPacketSent += PacketSent;
            _client.OnDisconnected += () => { _loggedIn = false; On.Trigger("d"); };
            _client.OnConnectFailed += () => { _loggedIn = false; On.Trigger("cf"); };
            _client.OnConnected += () =>
            {
                if (_registering)
                {
                    //Do Register
                    return;
                }
                _client.WritePacket(PacketTemplates.Login(Settings.Email, Settings.Device.GetStrDevice(), "2.8.1, 60842", !Redirect, Settings.SpamFilter));
                On.Trigger("c");
            };
        }

        private void PacketSent(Packet pkt)
        {
            if (PacketLog)
            {
                $"[{DateTime.Now.ToString()}] ^c=> ^w{pkt.Command.PadRight(24, ' ')} ^y{pkt.Payload?.Length}".ColouredConsole();
            }

            On.Trigger("ps", pkt);
        }

        private void PacketRecieved(Packet pkt)
        {
            _processor.Process(this, pkt, _client);
            if (PacketLog)
            {
                $"[{DateTime.Now.ToString()}] ^e<= ^w{pkt.Command.PadRight(24, ' ')} ^y{pkt.Payload?.Length}".ColouredConsole();
            }
            On.Trigger("pr", pkt);
        }

        /// <summary>
        /// Constructor for loading with a title
        /// </summary>
        /// <param name="title"></param>
        public PalBot(string title)
            : this()
        {
            Title = title;
        }

        /// <summary>
        /// Constructor for loading without plugins
        /// </summary>
        /// <param name="loadPlugins"></param>
        public PalBot(bool loadPlugins)
            : this()
        {
            _loadPlugins = loadPlugins;
        }

        /// <summary>
        /// Constructor for loading with title and without plugins
        /// </summary>
        /// <param name="title"></param>
        /// <param name="loadPlugins"></param>
        public PalBot(string title, bool loadPlugins)
            : this()
        {
            Title = title;
            _loadPlugins = loadPlugins;
        }

        /// <summary>
        /// Login to the system using email address, password, online status, device type, and spam filter options
        /// </summary>
        /// <param name="email">The email address for the bot's account</param>
        /// <param name="password">The password for the bot's account</param>
        /// <param name="auth">The Auth status (Online, offline, invisible) to sign in with</param>
        /// <param name="type">The device type to sign in with</param>
        /// <param name="spamfilter">Whether or not to apply spam filter</param>
        public void Login(string email, string password,
            AuthStatus auth = AuthStatus.Online, 
            DeviceType type = DeviceType.PC,
            bool spamfilter = false)
        {
            var settings = new Settings(email, password, auth, type, spamfilter, true);
            Login(settings);
        }

        /// <summary>
        /// Login with settings provided by the user
        /// </summary>
        /// <param name="settings">The settings</param>
        public void Login(Settings settings)
        {
            LoginPrelim();
            Settings = settings;
            _client.Start();
        }

        /// <summary>
        /// Login with preset settings
        /// </summary>
        public void Login()
        {
            if (Settings == null)
                throw new ArgumentException("You must set the settings before loggin in this way");
            LoginPrelim();
            _client.Start();
        }

        private void LoginPrelim()
        {
            MessageQueue = new MessageQueue(this, WatchForThrottle, _client);
            if (_loadPlugins)
            {
                Plugins = new PluginManager();
                Plugins.Start(this);
                _loadPlugins = false;
            }
        }

        /// <summary>
        /// Translate text that has been stored in the bot with a given key, and language
        /// Falls back onto <paramref name="def"/> if not translation exists 
        /// </summary>
        /// <param name="key">The key of the translation</param>
        /// <param name="language">The language to get the translation in</param>
        /// <param name="def">The default value to fall back on</param>
        /// <returns>The translated string</returns>
        public string Translate(string key, string language, string def = null)
        {
            if (language == null || key == null)
                return def;

            var trans = Translations[key]
                .Where(t => t.Language == language)
                .ToArray();

            if (trans.Length <= 0)
                return def;
            return trans[0].Value;
        }
    }
}
