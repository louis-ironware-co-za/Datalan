using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Net.Http;

namespace Coinigy.API.V2.EndPoints
{
    internal static class RequestHelper
    {
        public static string CalculateSignature(string apiSecret, string message)
        {
            if (string.IsNullOrEmpty(apiSecret))
            {
                throw new ArgumentNullException("Secret");
            }

            if (message == null)
            {
                message = "";
            }

            string toreturn = "";

            using (HMACSHA256 code = new HMACSHA256(ASCIIEncoding.ASCII.GetBytes(apiSecret)))
            {
                var hash = code.ComputeHash(ASCIIEncoding.ASCII.GetBytes(message));
                //toreturn = ASCIIEncoding.ASCII.GetString(hash);
                var hex = BitConverter.ToString(hash);
                toreturn = hex.Replace("-", "");
                
            }

            System.Diagnostics.Debug.WriteLine("Signature: " + toreturn);
            return toreturn;
        }

        private static FormUrlEncodedContent ConvertToContent(object obj)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            var objectJson = JsonConvert.SerializeObject(obj, settings);

            var dictionary = new Dictionary<string, string>();

            var objectDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(objectJson);

            foreach (var pair in objectDictionary)
            {
                dictionary.Add(key: pair.Key, value: pair.Value);
            }

            return new FormUrlEncodedContent(dictionary);
        }
    }
}
