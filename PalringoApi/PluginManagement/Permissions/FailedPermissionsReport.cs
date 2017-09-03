using PalringoApi.PacketData;

namespace PalringoApi.PluginManagement.Permissions
{
    /// <summary>
    /// Report to be generated when someone fails a permission engines test
    /// </summary>
    public class FailedPermissionsReport
    {
        /// <summary>
        /// The message that failed
        /// </summary>
        public Message Message { get; set; }
        /// <summary>
        /// The required permissions
        /// </summary>
        public Permission Required { get; set; }
        /// <summary>
        /// The plugin requested
        /// </summary>
        public string Requested { get; set; }
        /// <summary>
        /// The type of plugin
        /// </summary>
        public PluginType Type { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="req"></param>
        /// <param name="re"></param>
        /// <param name="t"></param>
        public FailedPermissionsReport(Message msg, Permission req, string re, PluginType t)
        {
            Message = msg;
            Required = req;
            Requested = re;
            Type = t;
        }
    }

    /// <summary>
    /// The type of plugins
    /// </summary>
    public enum PluginType
    {
        /// <summary>
        /// Simple plugins collection
        /// </summary>
        SimpleCollection = -1,
        /// <summary>
        /// Simple plugins
        /// </summary>
        Simple = 0,
        /// <summary>
        /// Backwards compatible plugins
        /// </summary>
        BackwardsCompatibleSimple = 1,
        /// <summary>
        /// Questionnaire plugins
        /// </summary>
        Questionnaire = 2,
        /// <summary>
        /// Forum Plugins
        /// </summary>
        Forum = 3,
        /// <summary>
        /// Stringable plugins
        /// </summary>
        Stringable = 4,
        /// <summary>
        /// User defined plugins
        /// </summary>
        UserDefined = 5
    }
}
