using PalringoApi.Plugins;

namespace PalringoApi.Test.PluginsTest
{
    [PluginCollection(">")]
    public class PluginTest : IPlugin
    {
        [Plugin("test")]
        public void Test(string message)
        {
            Reply("Up and running!");
        }
    }
}
