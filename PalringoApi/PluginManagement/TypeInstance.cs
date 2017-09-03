using System;

namespace PalringoApi.PluginManagement
{
    /// <summary>
    /// Holds an instance of a type (used for plugins)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TypeInstance<T>
    {
        /// <summary>
        /// The type to hold
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// The value to assign to the type
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// An optional language
        /// </summary>
        public string Language { get; set; } = null;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public TypeInstance(Type type, T value)
        {
            Type = type;
            Value = value;
        }
    }
}
