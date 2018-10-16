using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Common.Serialization
{
    /// <summary>
    /// Collection of utility functions to help parsing JSON formatted strings
    /// into corresponding objects preserving some sub-objects structure
    /// </summary>
    public static class JsonUtils
    {
        /// <summary>
        /// Parses input JSON string and returns the root object.
        /// Use GetTokenValue() to access a specific field within the JSON.
        /// </summary>
        /// <param name="json">JSON string for parsing.</param>
        /// <returns>Root JSON object.</returns>
        public static JToken GetRootToken(string json)
        {
            // Adding and then removing additional backslash allows us to keep
            // them in the resulting strings unmodified
            using (var reader = new JsonTextReader(new StringReader(json.Replace("\\", "\\\\"))))
            {
                // We do not parse date-time and interpret them as strings.
                reader.DateParseHandling = DateParseHandling.None;
                return JObject.Load(reader);
            }
        }

        /// <summary>
        /// Gets a string field from a JSON object using JPATH.
        /// </summary>
        /// <param name="token">Root JSON object.</param>
        /// <param name="jpath">JPATH for the field to retrieve.</param>
        /// <returns>The field string value.</returns>
        public static string GetTokenValue(JToken token, string jpath)
        {
            JToken t = token.SelectToken(jpath);
            return null != t ? t.ToString().Replace("\\\\", "\\") : null;
        }

        /// <summary>
        /// Gets an enumeration field from a JSON object using JPATH.
        /// </summary>
        /// <typeparam name="E">Enumeration type for the result.</typeparam>
        /// <param name="token">Root JSON object.</param>
        /// <param name="jpath">JPATH for the field to retrieve.</param>
        /// <returns>The field enumeration value.</returns>
        public static E GetTokenValue<E>(JToken token, string jpath)
        {
            token = token.SelectToken(jpath);
            JsonSerializer s = new JsonSerializer();
            s.Converters.Add(new StringEnumConverter());
            return token.ToObject<E>(s);
        }

        /// <summary>
        /// Retrieves the JSON formatted string of a sub-object within the root
        /// JSON object.
        /// </summary>
        /// <param name="token">Root JSON object.</param>
        /// <param name="jpath">JPATH for the sub-object to retrieve.</param>
        /// <returns>JSON string representation of the sub-object.</returns>
        public static string GetTokenJson(JToken token, string jpath)
        {
            token = token.SelectToken(jpath);
            return null != token ? token.ToString(Formatting.None).Replace("\\\\", "\\") : null;
        }
    }
}
