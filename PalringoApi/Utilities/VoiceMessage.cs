namespace PalringoApi.Utilities
{
    /// <summary>
    /// Handles Voice Message Creation for method overloads
    /// Do not instanciate, just do (VoiceMessage)byte[]
    /// </summary>
    public class VoiceMessage
    {
        /// <summary>
        /// The voice message data
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Coverts a byte[] to a VoiceMessage
        /// </summary>
        /// <param name="data">The data</param>
        public static implicit operator VoiceMessage(byte[] data)
        {
            return new VoiceMessage
            {
                Data = data
            };
        }

        /// <summary>
        /// Converts a VoiceMessage to a byte[]
        /// </summary>
        /// <param name="msg">The data</param>
        public static implicit operator byte[](VoiceMessage msg)
        {
            return msg.Data;
        }
    }
}
