using PalringoApi.PluginManagement.Permissions;
using PalringoApi.Subprofile.Types;
using System;

namespace PalringoApi.Plugins
{
    /// <summary>
    /// The information for a questionnaire plugin
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class Questionnaire : Attribute
    {
        /// <summary>
        /// The default Permissions engine to use.
        /// </summary>
        public static IPermissionEngine DefaultEngine = new PermissionEngine();

        /// <summary>
        /// The message that will trigger the plugin
        /// </summary>
        public string Trigger { get; private set; }

        /// <summary>
        /// Translation key for this plugin
        /// </summary>
        public string TranslationKey { get; set; } = null;

        /// <summary>
        /// The type of message type this plugin adheres to. (Defaults to all)
        /// </summary>
        public MessageType Type { get; set; } = MessageType.Group | MessageType.Private;

        /// <summary>
        /// The required permissions inorder to use the plugin (Defaults to All)
        /// </summary>
        public Permission Permissions { get; set; } = Permission.All;

        /// <summary>
        /// A Placeholder for the bot these plugins adhere to.
        /// If null or length is 0, it will be used for all bots.
        /// </summary>
        public string[] For { get; set; } = new string[0];

        /// <summary>
        /// A Description of the plugin used to build the help menu.
        /// </summary>
        public string Description { get; set; } = "";

        /// <summary>
        /// The format of the help menu
        /// Defaults to "{command character} {plugin name} - {description}"
        /// </summary>
        public string PluginHelpFormat { get; set; } = "{0} {1} - {2}";

        /// <summary>
        /// The type of permissions engine (defaults to <see cref="PermissionEngine"/>)
        /// </summary>
        public System.Type PermissionEngineType { get; set; } = null;

        /// <summary>
        /// Gets or sets the word used to cancel anytime during the questionnaire.
        /// </summary>
        public string CancelWord { get; set; } = null;

        /// <summary>
        /// Gets the instance of the permissions engine from <see cref="PermissionEngineType"/> (defaults to <see cref="PermissionEngine"/> 
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
        /// Default constructor for the Questionnaire
        /// </summary>
        /// <param name="trigger">The command that triggers the plugin</param>
        public Questionnaire(string trigger)
        {
            Trigger = trigger;
        }

        /// <summary>
        /// Preforms a deep clone of the questionnaire object
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public Questionnaire Clone(string trigger)
        {
            return new Questionnaire(trigger)
            {
                TranslationKey = TranslationKey,
                Type = Type,
                Permissions = Permissions,
                For = For,
                Description = Description,
                PluginHelpFormat = PluginHelpFormat,
                PermissionEngineType = PermissionEngineType,
                CancelWord = CancelWord
            };
        }
    }
}
