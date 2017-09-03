using PalringoApi.PacketData;

namespace PalringoApi.Plugins.Forums
{
    /// <summary>
    /// IIForum interface used for building the backend of IForums
    /// </summary>
    public interface IIForum
    {
        /// <summary>
        /// The bot instance
        /// </summary>
        PalBot Bot { get; set; }
        /// <summary>
        /// The message being sent
        /// </summary>
        Message Message { get; set; }
        /// <summary>
        /// The language of the instance
        /// </summary>
        string Language { get; set; }
        /// <summary>
        /// An instance of the attribute that triggered the forum
        /// </summary>
        Forum AttributeInstance { get; set; }
    }
}
