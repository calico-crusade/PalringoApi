namespace PalringoApi.PluginManagement
{
    /// <summary>
    /// Interface for plugin managers
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// What to do when loading the plugins
        /// </summary>
        /// <param name="bot">The bot instance to attach to</param>
        void Load(PalBot bot);
    }
}
