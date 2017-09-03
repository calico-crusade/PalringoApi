using System.Collections.Generic;
using System.Linq;

namespace PalringoApi.PluginManagement.TranslationHandling
{
    using Loaders;
    using Models;
    /// <summary>
    /// Holds translations for bot's use
    /// </summary>
    public class TranslationHolder
    {
        /// <summary>
        /// The type of loader to use
        /// </summary>
        public ILoader Loader { get; set; }

        /// <summary>
        /// The currently help translations
        /// </summary>
        public List<Translation> Translations { get; set; } = new List<Translation>();

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="load"></param>
        public TranslationHolder(ILoader load)
        {
            Loader = load;
            Translations = Loader.Load().ToList();
        }

        /// <summary>
        /// Adds a translation to the collection
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lang"></param>
        /// <param name="valu"></param>
        /// <returns></returns>
        public bool Add(string key, string lang, string valu)
        {
            return Add(new Translation(key, lang, valu));
        }

        /// <summary>
        /// Adds a translation to the collection
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        public bool Add(Translation trans)
        {
            if (Translations.Any(t => t.Key == trans.Key && t.Language == trans.Language))
                return false;

            Translations.Add(trans);
            return true;
        }

        /// <summary>
        /// Removes a translation from the collection
        /// </summary>
        /// <param name="trans"></param>
        public void Remove(Translation trans)
        {
            Translations.Remove(trans);
        }

        /// <summary>
        /// Saves a translation to the loader's save method
        /// </summary>
        public void Save()
        {
            Loader.Save(Translations);
        }

        /// <summary>
        /// Safe get of all the translations of a certain key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Translation[] this[string key]
        {
            get
            {
                return key == null ? new Translation[0] : Translations.Where(t => t.Key == key).ToArray();
            }
        }

        /// <summary>
        /// Load the translations from JSON
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static TranslationHolder FromJson(string filename)
        {
            return new TranslationHolder(new LoadFromJson(filename));
        }

        /// <summary>
        /// Load the translations from XML
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static TranslationHolder FromXml(string filename)
        {
            return new TranslationHolder(new LoadFromXml(filename));
        }

        /// <summary>
        /// Load the translations from an outside source
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static TranslationHolder FromOutside(Translation[] data)
        {
            return new TranslationHolder(new LoadFromOutside(data));
        }
    }
}
