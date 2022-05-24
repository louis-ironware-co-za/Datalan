using Coinigy.API.V2.Models;
using Coinigy.API.V2.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.EndPoints
{
    public class ExchangeAccounts
    {
        APIClient ApiClient { get; set; }

        public string ExchangeAccountsRelativeURL { get; private set; } = "/private/user/accounts";

        public ExchangeAccounts(APIClient client)
        {
            this.ApiClient = client;
            ExchangeAccountsRelativeURL = ApiClient.APIVersion + ExchangeAccountsRelativeURL;
        }

        public async Task<GetAccountsResponse> GetExchangeAccountsAsync()
        {
            string method = "GET";
            string endPoint = ExchangeAccountsRelativeURL;
            string query = "";

            var req = ApiClient.GetApiRequest();
            req.Method = method;
            req.Endpoint = endPoint;
            req.Body = query;

            return await ApiClient.ProcessGetAsync<GetAccountsResponse>(req);
        }

        public async Task<UserBalance> GetBalanceAsync(string authId)
        {
            string method = "GET";
            string endPoint = ExchangeAccountsRelativeURL + "?authId=" + authId;
            string query = "";

            var req = ApiClient.GetApiRequest();
            req.Method = method;
            req.Endpoint = endPoint;
            req.Body = query;

            return await ApiClient.ProcessGetAsync<UserBalance>(req);
        }
    }
}
