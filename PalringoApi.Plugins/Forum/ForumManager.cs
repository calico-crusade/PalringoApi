using PalringoApi.PacketData;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using PalringoApi.Subprofile.Types;
using PalringoApi.PluginManagement.Permissions;
using System.Threading;
using PalringoApi.PluginManagement;

namespace PalringoApi.Plugins.Forums
{
    /// <summary>
    /// The manager for Forum plugins
    /// </summary>
    public class ForumManager : IManager
    {
        /// <summary>
        /// All of the instances of the Group Forums currently active
        /// </summary>
        public Dictionary<int, Dictionary<int, Action<Message>>> GroupInstances { get; set; } = new Dictionary<int, Dictionary<int, Action<Message>>>();

        /// <summary>
        /// All of the instance of the private Forums currently active
        /// </summary>
        public Dictionary<int, Action<Message>> PrivateInstances { get; set; } = new Dictionary<int, Action<Message>>();

        /// <summary>
        /// A collection of all avialable Forum Plugins
        /// </summary>
        public Dictionary<System.Type, Dictionary<MethodInfo, List<Forum>>> Plugins { get; set; } = new Dictionary<System.Type, Dictionary<MethodInfo, List<Forum>>>();

        /// <summary>
        /// Initial load of the Forum Plugins
        /// </summary>
        /// <param name="bot"></param>
        public void Load(PalBot bot)
        {
            try
            {
                bot.On.GroupMessage += (m) => OnMessage(bot, m);
                bot.On.PrivateMessage += (m) => OnMessage(bot, m);

                var plugs = typeof(IIForum)
                    .GetAllTypes()
                    .Select(t => new KeyValuePair<System.Type, Dictionary<MethodInfo, List<Forum>>>(t, t
                        .GetMethods()
                        .Where(a => Attribute.IsDefined(a, typeof(Forum)))
                        .ToDictionary(b => b, b => Attribute.GetCustomAttributes(b, typeof(Forum)).Cast<Forum>().ToList())));

                foreach (var item in plugs)
                {
                    Plugins.Add(item.Key, new Dictionary<MethodInfo, List<Forum>>());

                    foreach (var Forum in item.Value)
                    {
                        Plugins[item.Key].Add(Forum.Key, new List<Forum>());

                        foreach (var atr in Forum.Value)
                        {
                            if (atr.For != null && bot.Title != null && !atr.For.Contains(bot.Title))
                                continue;

                            foreach (var alias in atr.Aliases)
                            {
                                Plugins[item.Key][Forum.Key].Add(atr.Clone(alias));
                            }

                            if (bot.Translations != null)
                            {
                                var trans = bot.Translations[atr.TranslationKey];
                                foreach (var t in trans)
                                {
                                    var c = atr.Clone(t.Value);
                                    c.Language = t.Language;
                                    Plugins[item.Key][Forum.Key].Add(c);
                                }
                            }

                            Plugins[item.Key][Forum.Key].Add(atr);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                bot.On.Trigger("e", ex);
            }
        }
        
        private void OnMessage(PalBot bot, Message msg)
        {
            try
            {
                if (msg.MesgType == MessageType.Group && GroupInstances.ContainsKey(msg.GroupId) && GroupInstances[msg.GroupId].ContainsKey(msg.SourceUser))
                {
                    
                    GroupInstances[msg.GroupId][msg.SourceUser](msg);
                    return;
                }
                if (msg.MesgType == MessageType.Private && PrivateInstances.ContainsKey(msg.SourceUser))
                {
                    PrivateInstances[msg.SourceUser](msg);
                    return;
                }
            }
            catch (Exception ex)
            {
                bot.On.Trigger("e", ex);
                return;
            }

            DoPluginSort(bot, msg);
        }

        private void DoPluginSort(PalBot bot, Message msg)
        {
            var group = msg.GroupId.GetGroup(bot);
            var user = msg.SourceUser.GetUser(bot);

            string m = msg.Content.Trim();
            foreach(var type in Plugins)
            {
                foreach(var meth in type.Value)
                {
                    var matching = meth.Value.Where(t => m.StartsWith(t.Trigger)).ToArray();

                    foreach(var Forum in matching)
                    {
                        if (!Forum.Type.HasFlag(msg.MesgType))
                            continue;
                        if (bot.Title != null && Forum.For != null && Forum.For.Length > 0 && !Forum.For.Contains(bot.Title))
                            continue;
                        if (!Forum.PermissionEngine.CheckPermissions(Forum.Permissions, user, group))
                        {
                            bot.On.Trigger("pf", bot, new FailedPermissionsReport(msg, Forum.Permissions, Forum.Trigger, PluginType.Forum));
                            continue;
                        }

                        var sm = m.Remove(0, Forum.Trigger.Length).Trim();

                        var i = (IIForum)Activator.CreateInstance(type.Key);
                        i.Bot = bot;
                        i.Language = Forum.Language;
                        i.Message = msg;
                        i.AttributeInstance = Forum;
                        if (msg.MesgType == MessageType.Group)
                        {
                            if (!GroupInstances.ContainsKey(msg.GroupId))
                                GroupInstances.Add(msg.GroupId, new Dictionary<int, Action<Message>>());
                            GroupInstances[msg.GroupId].Add(msg.SourceUser, (m2) => { });
                        }
                        else
                            PrivateInstances.Add(msg.SourceUser, (m2) => { });

                        new Thread(() =>
                        {
                            try
                            {
                                meth.Key.Invoke(i, new object[] { sm });
                                RemoveInstance(msg);
                            }
                            catch (Exception ex)
                            {
                                bot.On.Trigger("e", ex);
                            }
                        }).Start();
                    }
                }
            }
        }

        /// <summary>
        /// Removes an instance of a Forum based on the specifications of the Message supplied.
        /// </summary>
        /// <param name="msg"></param>
        public void RemoveInstance(Message msg)
        {
            if (msg.MesgType == MessageType.Group &&
                                    GroupInstances.ContainsKey(msg.GroupId) &&
                                    GroupInstances[msg.GroupId].ContainsKey(msg.SourceUser))
                GroupInstances[msg.GroupId].Remove(msg.SourceUser);
            else if (msg.MesgType == MessageType.Private &&
                PrivateInstances.ContainsKey(msg.SourceUser))
                PrivateInstances.Remove(msg.SourceUser);
        }
    }
}
