using PalringoApi.PluginManagement.TranslationHandling.Models;
using System.Collections.Generic;

namespace PalringoApi.PluginManagement.TranslationHandling.Loaders
{
    /// <summary>
    /// Interface for translation loaders
    /// </summary>
    public interface ILoader
    {
        /// <summary>
        /// The method that loads the translations
        /// </summary>
        /// <returns></returns>
        IEnumerable<Translation> Load();

        /// <summary>
        /// The method that saves the translations
        /// </summary>
        /// <param name="trans"></param>
        void Save(IEnumerable<Translation> trans);
    }
}
