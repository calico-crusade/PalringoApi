using System;

namespace PalringoApi.Subprofile.Types
{
    /// <summary>
    /// Data type of messages
    /// </summary>
    [Flags]
    public enum DataType
    {
        /// <summary>
        /// text/plain
        /// </summary>
        Text = 1,
        /// <summary>
        /// text/html
        /// </summary>
        RichMessage = 2,
        /// <summary>
        /// Audio/speex
        /// </summary>
        VoiceMessage = 4,
        /// <summary>
        /// image/jpeg
        /// </summary>
        Image = 8
    }
}
