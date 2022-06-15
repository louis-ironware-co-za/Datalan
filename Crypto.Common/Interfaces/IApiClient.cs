using Crypto.Common.Models;
using Crypto.Common.Requests;
using Crypto.Common.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Interfaces
{
    public abstract class IApiClient
    {
        public ICryptoExchange CryptoExchange { get; private set; }

        protected IApiClient(ICryptoExchange cryptoExchange)
        {
            CryptoExchange = cryptoExchange;
        }

        public abstract Task<List<CommonExchange>> GetExchangesAsync();

        public abstract Task<List<CommonMarket>> GetMarketsAsync();

        public abstract Task<List<CommonMarket>> GetMarketsAsync(CommonExchange exchange);

        public abstract Task<List<CommonUserOrderItem>> GetOrdersAsync();

        public abstract Task<List<CommonUserOrderItem>> GetOrdersAsync(CommonExchange exchange);

        public abstract Task<CommonNewOrderResponse> CreateMarketOrder(CommonNewOrderRequest request);
        public abstract Task<bool> CancelMarketOrder(CommonCancelOrderRequest request);
        public abstract Task<List<CommonAccountBalance>> GetAccountBalancesAsync();
    }
}
