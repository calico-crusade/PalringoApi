using PalringoApi.PluginManagement;
using PalringoApi.Plugins.Forums;
using PalringoApi.Plugins.Questionnaires;
using PalringoApi.Plugins.Simple;
using PalringoApi.Plugins.Stringable;
using System;

namespace PalringoApi.Plugins
{
    public static class PluginUtilities
    {
        public static T GetManager<T>(this PalBot bot) where T : IManager
        {
            foreach(var p in bot.Plugins.Triggered)
            {
                if (p is T)
                    return (T)p;
            }

            throw new ArgumentException("Invalid Manager Type!");
        }

        public static QuestionnaireManager QMan (this PalBot bot)
        {
            return bot.GetManager<QuestionnaireManager>();
        }

        public static ForumManager FMan(this PalBot bot)
        {
            return bot.GetManager<ForumManager>();
        }

        public static SimplePluginManager SMan(this PalBot bot)
        {
            return bot.GetManager<SimplePluginManager>();
        }

        public static Commands CMan(this PalBot bot) 
        {
            return bot.GetManager<Commands>();
        }
    }
}
