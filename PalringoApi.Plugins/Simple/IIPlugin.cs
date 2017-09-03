using PalringoApi.PacketData;

namespace PalringoApi.Plugins.Simple
{
    /// <summary>
    /// Interface for handling IPlugins
    /// </summary>
    public interface IIPlugin
    {
        /// <summary>
        /// Instance of all of the plugins in the current namepsace
        /// </summary>
        PluginCollection CollectionInstance { get; set; }

        /// <summary>
        /// Collection of all the Plugins relating to this item
        /// </summary>
        Plugin[] Plugins { get; set; }

        /// <summary>
        /// The current message
        /// </summary>
        Message Message { get; set; }

        /// <summary>
        /// The current Bot instance
        /// </summary>
        PalBot Bot { get; set; }

        /// <summary>
        /// An Object state for advanced users
        /// </summary>
        object State { get; set; }

        /// <summary>
        /// The language the plugin was start in (If using translations)
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// Method to generate help text
        /// </summary>
        /// <returns>Generated help text</returns>
        string HelpText();
    }
}
