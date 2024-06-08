using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AaTools.DataLayer
{
    internal class AaDataLayerHelper
    {
        /// <summary>
        ///  Deserialize Json to an object
        /// </summary>
        /// <typeparam name="T">Type of object to be returned</typeparam>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static T DeserializeJSON<T>(string Json) where T : class
        {
#if ONLY_DEBUG
            _logger.Debug("DeserializeJSON<T> {typeof(T)}", typeof(T));
#endif
#if NET8_0_OR_GREATER
            return System.Text.Json.JsonSerializer.Deserialize<T>(Json);
#else
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Json);
#endif
        }

        /// <summary>
        ///  Deserialize Json to an object
        /// </summary>
        /// <typeparam name="T">Type of object to be returned</typeparam>
        /// <param name="Json"></param>
        /// <returns></returns>
        public static T DeserializeJSONIgnoreCase<T>(string Json) where T : class
        {
#if ONLY_DEBUG
            _logger.Debug("DeserializeJSONIgnoreCase<T> {typeof(T)}", typeof(T));
#endif
#if NET8_0_OR_GREATER
            return System.Text.Json.JsonSerializer.Deserialize<T>(Json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
#else
            // Newtownsoft is case insensitive by default
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Json);
#endif
        }

        /// <summary>
        /// Takes a object and returns it in JSON format
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static string SerializeJSON(object Data)
        {
#if ONLY_DEBUG
            _logger.Debug("SerializeJSON {Data}");
#endif
#if NET8_0_OR_GREATER
            return System.Text.Json.JsonSerializer.Serialize(Data);
#else
            return Newtonsoft.Json.JsonConvert.SerializeObject(Data);
#endif
        }
    }
}
