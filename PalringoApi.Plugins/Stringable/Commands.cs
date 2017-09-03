using PalringoApi.PacketData;
using PalringoApi.PluginManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PalringoApi.Plugins.Stringable
{
    /// <summary>
    /// Manager from stringable commands
    /// </summary>
    public class Commands : IManager
    {
        private List<CommandHolder> Plugs { get; set; } = new List<CommandHolder>();

        private class CommandHolder
        {
            public Func<PalBot, Message, bool> Matches { get; set; }

            public Action<PalBot, Message> Run { get; set; }

            public CommandHolder() { }

            public CommandHolder(Action<PalBot, Message> run, Func<PalBot, Message, bool> m)
            {
                Matches = m;
                Run = run;
            }
        }

        /// <summary>
        /// Intial load of the plugins
        /// </summary>
        /// <param name="bot"></param>
        public void Load(PalBot bot)
        {
            Plugs = new List<CommandHolder>();
            bot.On.GroupMessage += (gm) => OnMessage(bot, gm.Clone());
            bot.On.PrivateMessage += (pm) => OnMessage(bot, pm.Clone());
        }

        private void OnMessage(PalBot bot, Message msg)
        {
            foreach (var plug in Plugs.ToArray().Where(t => t.Matches(bot, msg.Clone())))
                plug.Run(bot, msg.Clone());
        }

        /// <summary>
        /// Watches to see if a message starts with a certain command
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="run"></param>
        /// <returns></returns>
        public Commands StartsWith(string msg, Action<PalBot, Message> run)
        {
            Plugs.Add(new CommandHolder
            {
                Matches = (b, m) => m.Content.ToLower().Trim().StartsWith(msg.ToLower().Trim()),
                Run = run
            });
            return this;
        }

        /// <summary>
        /// watches to see if a message contains a certain command
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="run"></param>
        /// <returns></returns>
        public Commands Contains(string msg, Action<PalBot, Message> run)
        {
            Plugs.Add(new CommandHolder(run, (b, m) => m.Content.ToLower().Contains(msg.ToLower())));
            return this;
        }

        /// <summary>
        /// Watches to see if a message ends with a certain command
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="run"></param>
        /// <returns></returns>
        public Commands EndsWith(string msg, Action<PalBot, Message> run)
        {
            Plugs.Add(new CommandHolder
            {
                Matches = (b, m) => m.Content.ToLower().Trim().EndsWith(msg.ToLower().Trim()),
                Run = run
            });
            return this;
        }

        /// <summary>
        /// Watches to see if a message matches a certain cretira set by the user
        /// </summary>
        /// <param name="matchs"></param>
        /// <param name="run"></param>
        /// <returns></returns>
        public Commands Custom(Func<PalBot, Message, bool> matchs, Action<PalBot, Message> run)
        {
            Plugs.Add(new CommandHolder(run, matchs));
            return this;
        }
    }
}
