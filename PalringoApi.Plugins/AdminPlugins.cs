namespace PalringoApi.Plugins.MasterPlugins
{
    /// <summary>
    /// Admin plugins designed by Alec
    /// </summary>
    [PluginCollection("~/")]
    public class AdminPlugin : IPlugin
    {

        private static string CreditText = @"Credits:
    Networking: Alec
    Encryption: D.J. Bernstein (http://code.logos.com/blog/code/Salsa20.cs)
    Simple Plugin System:
        Concept: Mike
        Remastered and improved: Alec
    Everything else:
        Original: Java PC Client (Palringo) &| WireShark
        Ported: Alec";

        /// <summary>
        /// The credits method
        /// </summary>
        /// <param name="message"></param>
        [Plugin("credits", Description = "Shows the Bot Credits")]
        public void Credits(string message)
        {
            Reply(CreditText);
        }
    }
}