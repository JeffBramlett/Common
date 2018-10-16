using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Serialization
{
    /// <summary>
    /// Static helper class for Json Serialize and Deserialize
    /// </summary>
    public static class JsonHelpers
    {
        /// <summary>
        /// Generic serialize for any object
        /// </summary>
        /// <typeparam name="T">the type of the object</typeparam>
        /// <param name="objectToSerialize">the object to serialize</param>
        /// <returns></returns>
        public static string Serialize<T>(T objectToSerialize) where T : class
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                //TypeNameHandling = TypeNameHandling.All,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            return JsonConvert.SerializeObject(objectToSerialize, settings);
        }

        /// <summary>
        /// Try to deserialize the string content into an object of type T
        /// </summary>
        /// <typeparam name="T">the type of the object</typeparam>
        /// <param name="content">the string to deserialize</param>
        /// <param name="resultObject">the object from deserialization</param>
        /// <returns>true if successful, false otherwise (resultObject is null)</returns>
        public static bool Deserialize<T>(string content, out T resultObject) where T:class
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            resultObject = JsonConvert.DeserializeObject<T>(content, settings);
            return true;
        }

    }
}
