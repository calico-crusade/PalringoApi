using System.Collections.Generic;
using System.IO;
using PalringoApi.PluginManagement.TranslationHandling.Models;

namespace PalringoApi.PluginManagement.TranslationHandling.Loaders
{
    /// <summary>
    /// Loads translations from JSON
    /// </summary>
    public class LoadFromJson : ILoader
    {
        private string Filename { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="file"></param>
        public LoadFromJson(string file)
        {
            Filename = file;
        }

        /// <summary>
        /// Load from JSON
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Translation> Load()
        {
            if (!File.Exists(Filename))
                return new Translation[0];

            var data = File.ReadAllText(Filename);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Translation[]>(data);
        }

        /// <summary>
        /// Save in json
        /// </summary>
        /// <param name="trans"></param>
        public void Save(IEnumerable<Translation> trans)
        {
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(trans);
            File.WriteAllText(Filename, data);
        }
    }
}
