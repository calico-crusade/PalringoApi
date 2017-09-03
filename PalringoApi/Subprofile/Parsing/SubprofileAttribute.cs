using System;

namespace PalringoApi.Subprofile.Parsing
{
    /// <summary>
    /// Attribute for marking a property as being avialable to be parsed
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SubprofileAttribute : Attribute
    {
        /// <summary>
        /// The tags the represent the property in the datamap
        /// </summary>
        public string[] Tag { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="tag"></param>
        public SubprofileAttribute(params string[] tag)
        {
            Tag = tag;
        }
    }
}
