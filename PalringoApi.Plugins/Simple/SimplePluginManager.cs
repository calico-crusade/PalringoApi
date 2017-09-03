using PalringoApi.PacketData;
using PalringoApi.PluginManagement;
using PalringoApi.PluginManagement.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace PalringoApi.Plugins.Simple
{
    /// <summary>
    /// Manager for simple plugins
    /// </summary>
    public class SimplePluginManager : IManager
    {
        /// <summary>
        /// Instance of all of the plugins available
        /// </summary>
        public Dictionary<TypeInstance<PluginCollection>, List<MethodInstance<Plugin>>> Plugins { get; set; } = new Dictionary<TypeInstance<PluginCollection>, List<MethodInstance<Plugin>>>();

        /// <summary>
        /// Called to load plugins
        /// </summary>
        /// <param name="bot"></param>
        public void Load(PalBot bot)
        {
            bot.On.GroupMessage += (gm) => OnMessage(bot, gm.Clone());
            bot.On.PrivateMessage += (pm) => OnMessage(bot, pm.Clone());

            if (bot.Translations == null)
            {
                Plugins = typeof(IIPlugin)
                    .GetAllTypes()
                    .Where(t => Attribute.IsDefined(t, typeof(PluginCollection)))
                    .Select(t => new TypeInstance<PluginCollection>(t, t.GetCustomAttribute<PluginCollection>()))
                    .ToDictionary(t => t, t => PluginsFromCollection(bot, t));
                return;
            }

            var plugins = typeof(IIPlugin).GetAllTypes()
                .Where(t => Attribute.IsDefined(t, typeof(PluginCollection)))
                .Select(t => new TypeInstance<PluginCollection>(t, t.GetCustomAttribute<PluginCollection>()))
                .ToArray();

            Plugins = plugins.Where(t => t.Value.TranslationKey == null)
                .ToDictionary(t => t, t => PluginsFromCollection(bot, t));

            var other = plugins.Where(t => !Plugins.Keys.Contains(t)).ToArray();

            foreach(var collection in other)
            {
                var trans = bot.Translations[collection.Value.TranslationKey];

                foreach(var item in trans)
                {
                    var clone = new TypeInstance<PluginCollection>(collection.Type, collection.Value.Clone(item.Value));
                    clone.Language = item.Language;
                    Plugins.Add(clone, PluginsFromCollection(bot, clone, item.Language));
                }

                Plugins.Add(collection, PluginsFromCollection(bot, collection, null));
            }
        }

        private List<MethodInstance<Plugin>> PluginsFromCollection(PalBot bot, TypeInstance<PluginCollection> col, string language = null)
        {
            return col.Type.GetMethods()
                .Where(t => Attribute.IsDefined(t, typeof(Plugin)))
                .Select(b => 
                {
                    var m = new MethodInstance<Plugin>(b, b.GetCustomAttribute<Plugin>());
                    m.Language = language;
                    if (m.Language == null || m.Value.TranslationKey == null)
                        return m;
                    var trans = bot.Translations[m.Value.TranslationKey].
                        Where(t => t.Language == m.Language).ToArray();
                    if (trans.Length <= 0)
                        return m;

                    var x = trans[0];

                    m.Value.Trigger = x.Value;

                    return m;
                }).ToList();
        }

        private void OnMessage(PalBot bot, Message message)
        {
            var group = message.GroupId.GetGroup(bot);
            var user = message.SourceUser.GetUser(bot);

            string given = message.Content;

            foreach (var p in Plugins)
            {
                var col = p.Key.Value;
                if (!col.Type.HasFlag(message.MesgType))
                    continue;

                if (bot.Title != null && col.For != null && col.For.Length > 0 && !col.For.Contains(bot.Title))
                    continue;

                var k = col.Trigger;
                if (!given.ToLower().StartsWith(k))
                    continue;

                if (!col.PermissionEngine.CheckPermissions(col.Permissions, user, group))
                    continue;

                var submsg = given.Remove(0, col.Trigger.Length).Trim();

                foreach (var m in p.Value)
                {
                    var plug = m.Value;
                    if (!plug.Type.HasFlag(message.MesgType) ||
                        !submsg.ToLower().StartsWith(plug.Trigger.ToLower()))
                        continue;

                    if (!col.PermissionEngine.CheckPermissions(plug.Permissions, user, group))
                    {
                        bot.On.Trigger("pf", bot, new FailedPermissionsReport(message, col.Permissions, col.Trigger + " " + plug.Trigger, PluginType.Simple));
                        continue;
                    }

                    var ssm = submsg.Remove(0, plug.Trigger.Length).Trim();

                    try
                    {
                        var i = Activator.CreateInstance(p.Key.Type) as IIPlugin;
                        i.CollectionInstance = col;
                        i.Plugins = p.Value.Select(t => t.Value).ToArray();
                        i.Message = message;
                        i.Language = p.Key.Language;
                        i.Bot = bot;
                        new Thread(() =>
                        {
                            m.Method.Invoke(i, new object[] { ssm.GetPalringoString() });
                        }).Start();
                    }
                    catch (Exception ex)
                    {
                        bot.On.Trigger("e", ex);
                    }
                }
            }
        }
    }
}
