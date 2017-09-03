using System;
using System.Collections.Generic;
using PalringoApi.PluginManagement.TranslationHandling.Models;

namespace PalringoApi.PluginManagement.TranslationHandling.Loaders
{
    /// <summary>
    /// Loads translations from an outside source (DB)
    /// </summary>
    public class LoadFromOutside : ILoader
    {
        private Translation[] Data { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="data"></param>
        public LoadFromOutside(Translation[] data)
        {
            Data = data;
        }

        /// <summary>
        /// Load the data from the source
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Translation> Load()
        {
            return Data;
        }

        /// <summary>
        /// Save the data (not do able)
        /// </summary>
        /// <param name="trans"></param>
        public void Save(IEnumerable<Translation> trans)
        {
            throw new Exception("Cannot save to outside source.");
        }
    }
}
