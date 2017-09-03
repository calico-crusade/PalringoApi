using System.Reflection;

namespace PalringoApi.PluginManagement
{
    /// <summary>
    /// Holder for a method and some other class (used by plugin handlers) also contains a language
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MethodInstance<T>
    {
        /// <summary>
        /// The method to hold
        /// </summary>
        public MethodInfo Method { get; set; }

        /// <summary>
        /// The class value to hold
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// The language for the plugin
        /// </summary>
        public string Language { get; set; } = null;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="method"></param>
        /// <param name="value"></param>
        public MethodInstance(MethodInfo method, T value)
        {
            Method = method;
            Value = value;
        }
    }
}
