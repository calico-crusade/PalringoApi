namespace PalringoApi.Subprofile.Types
{
    /// <summary>
    /// Whether a response is a code or a message
    /// </summary>
    public enum Type
    {
        /// <summary>
        /// Its a message
        /// </summary>
        Code = 0,
        /// <summary>
        /// It is definitely a code. I swear.
        /// </summary>
        Message = 1
    }
}
