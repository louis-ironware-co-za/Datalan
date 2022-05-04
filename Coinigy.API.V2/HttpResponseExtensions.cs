using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2
{
    public static class HttpResponseExtensions
    {
        public static T Deserialize<T>(this HttpResponseMessage response)
        {
            var settings = new JsonSerializerSettings();
            settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;

            var stream = response.Content.ReadAsStringAsync().Result;
            var item = JsonConvert.DeserializeObject<T>(stream, settings);

            return item;
        }

        public static bool TryDeserialize<T>(this HttpResponseMessage response, out T result)
        {
            result = default;
            var settings = new JsonSerializerSettings();
            settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            settings.MissingMemberHandling = MissingMemberHandling.Error;

            try
            {
                var stream = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<T>(stream, settings);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static string TryReadContent(this HttpResponseMessage response)
        {
            string message = string.Empty;

            try
            {
                message = response.Content.ReadAsStringAsync().Result;
            }
            catch { }

            return message;
        }
    }
}
