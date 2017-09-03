namespace PalringoApi.Subprofile.Parsing
{
    /// <summary>
    /// Can be parsed using SubProfile attribute
    /// Good idea, but never implemented
    /// </summary>
    public interface IParsable
    {
        /// <summary>
        /// Parse the information
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="data"></param>
        void Parse(PalBot bot, DataMap data);
    }
}
