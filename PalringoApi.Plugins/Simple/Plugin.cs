using PalringoApi.Subprofile.Types;
using System;

namespace PalringoApi.Plugins
{
    /// <summary>
    /// Attribute for marking methods as plugin entry points
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Plugin : Attribute
    {
        /// <summary>
        /// The trigger for the plugin
        /// </summary>
        public string Trigger { get; set; }

        /// <summary>
        /// The translation key for the plugin trigger
        /// </summary>
        public string TranslationKey { get; set; } = null;

        /// <summary>
        /// Whether or not this should be actived by private, group or either message types
        /// </summary>
        public MessageType Type { get; set; } = MessageType.Group | MessageType.Private;

        /// <summary>
        /// The permissions required to use this plugin
        /// </summary>
        public Permission Permissions { get; set; } = Permission.All;

        /// <summary>
        /// The description of the method
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// Default constructor for the plugin
        /// </summary>
        /// <param name="trigger"></param>
        public Plugin(string trigger)
        {
            Trigger = trigger;
        }
    }
}
