using PalringoApi.PluginManagement.Permissions;
using PalringoApi.Subprofile.Types;
using System;

namespace PalringoApi.Plugins
{
    /// <summary>
    /// Attribute that marks a class has having plugins
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginCollection : Attribute
    {
        /// <summary>
        /// The default plugin engine to be used by any PluginCollection in this application
        /// </summary>
        public static IPermissionEngine DefaultEngine = new PermissionEngine();

        /// <summary>
        /// The trigger for the plugin
        /// </summary>
        public string Trigger { get; private set; }

        /// <summary>
        /// The translation key for the plugin collections trigger
        /// </summary>
        public string TranslationKey { get; set; } = null;

        /// <summary>
        /// The type of message it will except (group / private / both)
        /// </summary>
        public MessageType Type { get; set; } = MessageType.Group | MessageType.Private;

        /// <summary>
        /// Permissions required to use any of the plugins in this collection
        /// </summary>
        public Permission Permissions { get; set; } = Permission.All;

        /// <summary>
        /// Matches up to the <see cref="PalBot.Title"/> to mark the plugins exclusively for that type
        /// Leave blank to match all blank plugins
        /// </summary>
        public string[] For { get; set; } = new string[0];

        /// <summary>
        /// The description of the set of plugins
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// The help format for the plugin helptext creator to use
        /// </summary>
        public string PluginHelpFormat { get; set; } = "{0} {1} - {2}";

        /// <summary>
        /// The permissions engine for this plugin collection
        /// </summary>
        public System.Type PermissionEngineType { get; set; } = null;

        /// <summary>
        /// Auto property for the permissions engine
        /// </summary>
        public IPermissionEngine PermissionEngine
        {
            get
            {
                try
                {
                    var type = typeof(IPermissionEngine);
                    if (!type.IsAssignableFrom(PermissionEngineType))
                        return DefaultEngine;
                    return (IPermissionEngine)Activator.CreateInstance(PermissionEngineType);
                }
                catch
                {
                    return DefaultEngine;
                }
            }
        }

        /// <summary>
        /// Default constructor for the plugincollection
        /// </summary>
        /// <param name="trigger"></param>
        public PluginCollection(string trigger)
        {
            Trigger = trigger;
        }

        /// <summary>
        /// Preforms a deep clone of the plugin collection
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public PluginCollection Clone(string trigger)
        {
            return new PluginCollection(trigger)
            {
                TranslationKey = TranslationKey,
                Type = Type,
                Permissions = Permissions,
                For = For,
                Description = Description,
                PluginHelpFormat = PluginHelpFormat,
                PermissionEngineType = PermissionEngineType
            };
        }
    }
}
