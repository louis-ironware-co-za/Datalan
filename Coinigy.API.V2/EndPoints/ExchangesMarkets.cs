using Coinigy.API.V2.Models;
using Coinigy.API.V2.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.EndPoints
{
    public class ExchangesMarkets
    {
        APIClient APIClient { get; set; }

        public string ExchangeRelativeURL { get; private set; } = "/private/exchanges";
        public string MarketRelativeURL { get; private set; } = "/private/markets";
        public string PublicMarketRelativeURL { get; private set; } = "/public/markets";

        public ExchangesMarkets(APIClient client)
        {
            APIClient = client;
            ExchangeRelativeURL = APIClient.APIVersion + ExchangeRelativeURL;
            MarketRelativeURL = APIClient.APIVersion + MarketRelativeURL;
            PublicMarketRelativeURL = APIClient.APIVersion + PublicMarketRelativeURL;
        }

        /// <summary>
        /// All Exchanges listed on Coinigy
        /// </summary>
        /// <returns></returns>
        public async Task<GetExchangesResponse> GetExchangesAsync()
        {
            string method = "GET";
            string endPoint = ExchangeRelativeURL;
            string query = "";

            var req = APIClient.GetApiRequest();
            req.Method = method;
            req.Endpoint = endPoint;
            req.Body = query;

            return await APIClient.ProcessGetAsync<GetExchangesResponse>(req);
        }

        /// <summary>
        /// All trading pairs listed on Coinigy
        /// </summary>
        /// <returns></returns>
        public async Task<GetMarketsResponse> GetMarketsAsync()
        {
            var req = APIClient.GetApiRequest();
            req.Method = "GET";
            req.Endpoint = MarketRelativeURL;

            return await APIClient.ProcessGetAsync<GetMarketsResponse>(req);
        }

        public async Task<GetPublicMarketsResponse> GetMarketsAsync(string exchange)
        {
            var req = APIClient.GetApiRequest();
            req.Method = "GET";
            req.Endpoint = PublicMarketRelativeURL + "?exchange=" + exchange;
            return await APIClient.ProcessGetAsync<GetPublicMarketsResponse>(req);
        }
    }
}
