using PalringoApi.PacketData;
using Newtonsoft.Json;
using PalringoApi.PluginManagement;
using PalringoApi.PluginManagement.Permissions;
using PalringoApi.Plugins.Simple;

namespace PalringoApi.Plugins
{
    /// <summary>
    /// IIPlugin instance with helper methods to make things easier
    /// </summary>
    public abstract class IPlugin : SendersInstance, IIPlugin
    {
        /// <summary>
        /// Instance of the higher level collections attribute that triggered this plugin
        /// </summary>
        [JsonIgnore]
        public PluginCollection CollectionInstance { get; set; }

        /// <summary>
        /// The Language the current plugin was called in
        /// </summary>
        [JsonIgnore]
        public override string Language { get; set; }

        /// <summary>
        /// Instance of all of the plugins in the current collection
        /// </summary>
        [JsonIgnore]
        public Plugin[] Plugins { get; set; }

        /// <summary>
        /// A state object to be used a users discretion (its useless to me :D)
        /// </summary>
        [JsonIgnore]
        public object State { get; set; }
        
        /// <summary>
        /// Creates help text based on the "Description" attributes for each method
        /// </summary>
        /// <returns></returns>
        public virtual string HelpText()
        {
            var user = Message.SourceUser.GetUser(Bot);
            var group = Message.GroupId.GetGroup(Bot);
            var text = CollectionInstance.Description;

            foreach(var p in Plugins)
            {
                if (!CollectionInstance.PermissionEngine.CheckPermissions(p.Permissions, user, group))
                    continue;
                text += string.Format("\r\n" + CollectionInstance.PluginHelpFormat,
                    CollectionInstance.Trigger,
                    p.Trigger, p.Description);
            }

            return text;
        }
    }
}
