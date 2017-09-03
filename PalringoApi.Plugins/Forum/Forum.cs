using PalringoApi.PluginManagement.Permissions;
using PalringoApi.Subprofile.Types;
using System;

namespace PalringoApi.Plugins
{
    /// <summary>
    /// Forum type plugins, a mesh between Questionnaires and SimplePlugins
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class Forum : Attribute
    {
        /// <summary>
        /// The default permissions engine to be used whenever an engine isn't supplyed
        /// </summary>
        public static IPermissionEngine DefaultEngine = new PermissionEngine();
        /// <summary>
        /// The trigger that causes the plugin to be rum
        /// </summary>
        public string Trigger { get; private set; }
        /// <summary>
        /// Aliases for the trigger
        /// </summary>
        public string[] Aliases { get; set; } = new string[0];
        /// <summary>
        /// The translation key for the Forum trigger
        /// </summary>
        public string TranslationKey { get; set; } = null;
        /// <summary>
        /// Whether to use in group or private or both
        /// </summary>
        public MessageType Type { get; set; } = MessageType.Group | MessageType.Private;
        /// <summary>
        /// The language (don't set, it will be overriden)
        /// </summary>
        public string Language { get; set; } = null;
        /// <summary>
        /// What bots can use this plugin, leave blank for any bot with no title
        /// Only good if you have multiple bots in one application
        /// </summary>
        public string[] For { get; set; } = new string[0];
        /// <summary>
        /// Description of what the forum does
        /// </summary>
        public string Description { get; set; } = "";
        /// <summary>
        /// The permissions required to use the plugin
        /// </summary>
        public Permission Permissions { get; set; } = Permission.All;
        /// <summary>
        /// The type of permissions engine to use for the permissions
        /// </summary>
        public System.Type PermissionEngineType { get; set; } = null;
        /// <summary>
        /// Auto proptery to get the permissions engine
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
        /// Default constructor for Forums
        /// </summary>
        /// <param name="trigger"></param>
        public Forum(string trigger)
        {
            Trigger = trigger;
        }

        /// <summary>
        /// Preforms a deep clone of the forum 
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public Forum Clone(string trigger)
        {
            return new Forum(trigger)
            {
                Aliases = Aliases,
                TranslationKey = TranslationKey,
                Type = Type,
                Language = Language,
                For = For,
                Description = Description,
                Permissions = Permissions,
                PermissionEngineType = PermissionEngineType
            };
        }
    }
}
