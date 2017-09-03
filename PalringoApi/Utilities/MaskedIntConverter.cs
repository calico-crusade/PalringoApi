using Newtonsoft.Json;

namespace PalringoApi.Utilities
{
    /// <summary>
    /// Converts UserId and GroupId classes into json parsable objects
    /// </summary>
    public class MaskedIntConverter : JsonConverter
    {
        /// <summary>
        /// If it can convert the object
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(UserId) || objectType == typeof(GroupId);
        }

        /// <summary>
        /// reads the json
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(UserId))
                return (UserId)(string)reader.Value;

            return (GroupId)(string)reader.Value;
        }

        /// <summary>
        /// writes the json
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
