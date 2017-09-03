using System;
using System.Collections.Generic;
using System.Linq;

namespace PalringoApi.PluginManagement
{
    /// <summary>
    /// Container for all individual plugin type managers
    /// </summary>
    public class PluginManager
    {

        /// <summary>
        /// Triggered Plugin Managers.
        /// </summary>
        public IManager[] Triggered => _triggered.ToArray();

        private IManager[] PluginHandlers => typeof(IManager).GetAllTypes().Select(t => Activator.CreateInstance(t) as IManager).ToArray();
        private List<IManager> _triggered { get; set; }

        /// <summary>
        /// What to do on loading the plugins
        /// </summary>
        /// <param name="bot">Bot to attach the instance to</param>
        public void Start(PalBot bot)
        {
            _triggered = new List<IManager>();
            try
            {
                foreach (var m in PluginHandlers)
                {
                    try
                    {
                        m.Load(bot);
                        bot.On.Trigger("ll", $"[{m.GetType().Name}] Loaded");
                        _triggered.Add(m);
                    }
                    catch (Exception ex)
                    {
                        bot.On.Trigger("e", $"[{m.GetType().Name}] Error => {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                bot.On.Trigger("e", $"Plugin Handlers failed to load: " + ex.Message);
            }
        }
    }
}
