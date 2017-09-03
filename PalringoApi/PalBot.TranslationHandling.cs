using PalringoApi.PluginManagement.TranslationHandling;
using PalringoApi.PluginManagement.TranslationHandling.Models;
using System.Collections.Generic;

namespace PalringoApi
{
    public partial class PalBot
    {
        /// <summary>
        /// Holder for translations 
        /// </summary>
        public TranslationHolder Translations { get; private set; } = null;

        /// <summary>
        /// Set up translations from an existing translations holder (aka another bot)
        /// </summary>
        /// <param name="trans"></param>
        public void SetupTranslations(TranslationHolder trans)
        {
            Translations = trans;
        }

        /// <summary>
        /// Set up translations from a JSON file
        /// </summary>
        /// <param name="filename"></param>
        public void SetupTranslationsFromJson(string filename)
        {
            Translations = TranslationHolder.FromJson(filename);
        }

        /// <summary>
        /// Set up translations from an XML file
        /// </summary>
        /// <param name="filename"></param>
        public void SetupTranslationsFromXml(string filename)
        {
            Translations = TranslationHolder.FromXml(filename);
        }

        /// <summary>
        /// Set up translations from an outside source.
        /// </summary>
        /// <param name="list"></param>
        public void SetupTranslationsFromOutside(List<Translation> list)
        {
            Translations = TranslationHolder.FromOutside(list.ToArray());
        }
    }
}
