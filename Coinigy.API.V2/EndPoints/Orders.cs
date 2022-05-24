using Coinigy.API.V2.Models;
using Coinigy.API.V2.Requests;
using Coinigy.API.V2.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.EndPoints
{
    public class Orders
    {
        APIClient APIClient { get; set; }

        public string UserAccountsRelativeUrl { get; private set; } = "/private/user/accounts";
        public string OrdersRelativeUrl { get; private set; } = "/private/user/orders";

        public Orders(APIClient client)
        {
            APIClient = client;
            UserAccountsRelativeUrl = APIClient.APIVersion + UserAccountsRelativeUrl;
            OrdersRelativeUrl = APIClient.APIVersion + OrdersRelativeUrl;
        }

        public async Task<UserOrderResponse> PlaceNewOrder(NewOrderRequest request, string authId)
        {
            ApiRequest req = APIClient.GetApiRequest();
            req.Method = "POST";
            req.Endpoint = UserAccountsRelativeUrl + $"/{authId}/orders";
            req.Body = JsonConvert.SerializeObject(request);

            return await APIClient.ProcessPostAsync<UserOrderResponse>(req, request);
        }

        public async Task<CoinigyResponse> CancelOrder(string authId, string orderId)
        {
            ApiRequest req = APIClient.GetApiRequest();
            req.Method = "DELETE";
            req.Endpoint = UserAccountsRelativeUrl + $"/{authId}/orders/{orderId}";

            return await APIClient.ProcessDeleteAsync<CoinigyResponse>(req);
        }

        /// <summary>
        /// Get open orders for a user
        /// </summary>
        public async Task<GetOrdersResponse> GetOrders()
        {
            ApiRequest req = APIClient.GetApiRequest();
            req.Method = "GET";
            req.Endpoint = OrdersRelativeUrl;

            return await APIClient.ProcessGetAsync<GetOrdersResponse>(req);
        }

        public async Task<GetOrdersResponse> GetOrders(string exchCode)
        {
            ApiRequest req = APIClient.GetApiRequest();
            req.Method = "GET";
            req.Endpoint = OrdersRelativeUrl + "?ExchCode=" + exchCode;

            return await APIClient.ProcessGetAsync<GetOrdersResponse>(req);
        }
    }
}
