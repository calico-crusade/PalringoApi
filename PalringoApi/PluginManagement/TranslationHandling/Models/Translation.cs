namespace PalringoApi.PluginManagement.TranslationHandling.Models
{
    /// <summary>
    /// A translation
    /// </summary>
    public class Translation
    {
        /// <summary>
        /// The key of the translation (cannot be duplicates)
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value of the translation
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The language the translation is in
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lang"></param>
        /// <param name="value"></param>
        public Translation(string key, string lang, string value)
        {
            Key = key;
            Language = lang;
            Value = value;
        }

        /// <summary>
        /// Another default constructor
        /// </summary>
        public Translation() { }
    }
}
