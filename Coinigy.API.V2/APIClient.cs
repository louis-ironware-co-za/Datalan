using Coinigy.API.V2.EndPoints;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2
{
    public class APIClient
    {
        string APIKey = "";
        string APISecret = "";

        public string BaseURL { get; set; } = "https://api.coinigy.com";
        public string APIVersion { get; set; } = "/api/v2";

        public PublicEP PublicEP { get; private set; }
        public ExchangesMarkets ExchangesMarkets { get; private set; }

        public APIClient(string key, string secret)
        {
            this.APIKey = key;
            this.APISecret = secret;

            SetupEndPoints();
        }

        private void SetupEndPoints()
        {
            ExchangesMarkets = new ExchangesMarkets(this);
        }

        public void AddDefaultHeaders(HttpClient client, ApiRequest request)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Context-Type", "application/json");
            client.DefaultRequestHeaders.Add("X-API-KEY", request.Key);
            client.DefaultRequestHeaders.Add("X-API-TIMESTAMP", request.Timestamp);
            client.DefaultRequestHeaders.Add("X-API-SIGN", request.Signature);
        }

        //public void Sign(HttpClient client, string message)
        //{
        //    if (string.IsNullOrEmpty(message))
        //    {
        //        throw new ArgumentNullException("message");
        //    }

        //    client.DefaultRequestHeaders.Add("X-API-SIGN", RequestHelper.CalculateSignature(APISecret, message));
        //}

        //public string GetMessage(string method, DateTime timestampNotUnix, string endPoint, string queryString, string body)
        //{
        //    if (queryString == null)
        //    {
        //        queryString = "";
        //    }

        //    if (body == null)
        //    {
        //        body = "";
        //    }

        //    string timeStamp = timestampNotUnix.UnixTimeFromDateTimeUTC().ToString();

        //    string msg = APIKey + timeStamp + method + endPoint + queryString + body;
        //    return msg;
        //}

        public HttpClient GetClient()
        {
            HttpClient client = new HttpClient()
            {
                BaseAddress = new Uri(BaseURL),
            };
            return client;
        }

        public ApiRequest GetApiRequest()
        {
            return new ApiRequest()
            {
                BaseUrl = BaseURL,
                Key = this.APIKey,
                Secret = this.APISecret,
            };
        }

        public async Task<T> ProcessGetAsync<T>(ApiRequest request) where T : class
        {
            using (HttpClient client = GetClient())
            {
                return await GetAsync<T>(client, request);
            }
        }

        public async Task<T> GetAsync<T>(HttpClient client, ApiRequest request) where T : class
        {
            AddDefaultHeaders(client, request);

            var requestResult = await client.GetAsync(request.Endpoint);
            if (requestResult.StatusCode == HttpStatusCode.OK)
            {
                return requestResult.Deserialize<T>();
            }

            var message = requestResult.TryReadContent();

            throw string.IsNullOrWhiteSpace(message) ? new Exception(requestResult.ReasonPhrase) : new Exception(requestResult.ReasonPhrase, new Exception(message));
        }

        public void TestSignature()
        {
            var offset = new DateTimeOffset(DateTime.Parse("2018-07-27T19:13:50.6694555Z"));

            ApiRequest req = new ApiRequest()
            {
                BaseUrl = "https://api.coinigy.com",
                Endpoint = "/api/v2/private/exchanges?pythagoreanTheorem=a%5E2%2Bb%5E2%3Dc%5E2",
                Key = "keykeykeykeykeykeykeykeykeykeyke",
                Secret = "secretsecretsecretsecretsecretse",
                Method = "GET",
                Timestamp = offset.ToUnixTimeSeconds().ToString(),

            };

            string shouldBe = "B618C0B3C92632C701D7CEFC00AC9C8A0771989B21E00D61D4945F79239D2F87";
            string current = req.Signature;

            System.Diagnostics.Debug.Assert(shouldBe == current, shouldBe + Environment.NewLine + current);
        }
    }
}
