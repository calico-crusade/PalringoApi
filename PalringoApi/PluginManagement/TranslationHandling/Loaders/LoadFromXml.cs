using System.Collections.Generic;
using System.Linq;
using System.IO;
using PalringoApi.PluginManagement.TranslationHandling.Models;
using System.Xml.Serialization;

namespace PalringoApi.PluginManagement.TranslationHandling.Loaders
{
    /// <summary>
    /// Load translations from an XML file
    /// </summary>
    public class LoadFromXml : ILoader
    {
        private string FileName { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filename"></param>
        public LoadFromXml(string filename)
        {
            FileName = filename;
        }

        /// <summary>
        /// Loads the translations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Translation> Load()
        {
            if (!File.Exists(FileName))
                return new Translation[0];

            using (var data = new MemoryStream(File.ReadAllBytes(FileName)))
            {
                var ser = new XmlSerializer(typeof(TranslationRoot));
                return ((TranslationRoot)ser.Deserialize(data)).Translations;
            }
        }

        /// <summary>
        /// Saves the translations
        /// </summary>
        /// <param name="trans"></param>
        public void Save(IEnumerable<Translation> trans)
        {
            using (var data = new MemoryStream())
            {
                var ser = new XmlSerializer(typeof(TranslationRoot));
                ser.Serialize(data, new TranslationRoot { Translations = trans.ToArray() });
                File.WriteAllBytes(FileName, data.ToArray());
            }
        }
    }

    /// <summary>
    /// XML based saving system
    /// </summary>
    [XmlRoot("TranslationCollection")]
    public class TranslationRoot
    {
        /// <summary>
        /// The translations to save
        /// </summary>
        [XmlElement("Translation")]
        public Translation[] Translations { get; set; }
    }
}
