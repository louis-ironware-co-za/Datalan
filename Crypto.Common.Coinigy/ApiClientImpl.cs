using Coinigy.API.V2;
using Coinigy.API.V2.Models;
using Crypto.Common.Coinigy.Conversion;
using Crypto.Common.Conversion;
using Crypto.Common.Interfaces;
using Crypto.Common.Models;
using Crypto.Common.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Coinigy
{
    public class ApiClientImpl : IApiClient
    {
        ExchangeImpl _exchange;
        APIClient Client { get => _exchange?.ActualExchange?.Client; }

        public ApiClientImpl(ExchangeImpl exchange) : base(exchange)
        {
            this._exchange = exchange;
        }

        public override Task<bool> CancelMarketOrder(CommonCancelOrderRequest request)
        {
            throw new NotImplementedException();
        }

        public override Task<CommonOrderItem> CreateMarketOrder(CommonNewOrderRequest request)
        {
            throw new NotImplementedException();
        }

        public override async Task<List<CommonExchange>> GetExchangesAsync()
        {
            var res = await Client.ExchangesMarkets.GetExchangesAsync();
            return res.Result.Select(x => x.ToCommonExchange()).ToList();
        }

        public override async Task<List<CommonMarket>> GetMarketsAsync()
        {
            var res = await Client.ExchangesMarkets.GetMarketsAsync();
            var toreturn = res.Result.Select(x => x.ToCommonMarket()).ToList();
            toreturn.ForEach(x => x.ExchangeRef = this.CryptoExchange);
            return toreturn;
        }

        public override async Task<List<CommonMarket>> GetMarketsAsync(CommonExchange exchange)
        {
            if (exchange == null || string.IsNullOrEmpty(exchange.ExchName))
            {
                throw new ArgumentException("Null exchange or exchange does not have a valid name.");
            }

            var res = await Client.ExchangesMarkets.GetMarketsAsync(exchange.ExchName);
            var toreturn = res.Result.Select(x => x.ToCommonMarket()).ToList();
            toreturn.ForEach(x => x.ExchangeRef = this.CryptoExchange);
            return toreturn;
        }

        public override async Task<List<CommonUserOrderItem>> GetOrdersAsync()
        {
            var res = await Client.Orders.GetOrders();
            var toreturn = res.Result.Select(x => x.ToCommonUserOrderItem()).ToList();
            toreturn.ForEach(x => x.ExchangeRef =this.CryptoExchange);
            return toreturn;
        }

        public override async Task<List<CommonUserOrderItem>> GetOrdersAsync(CommonExchange exchange)
        {
            if (exchange == null || string.IsNullOrEmpty(exchange.ExchCode))
            {
                throw new ArgumentException("Null exchange or exchange does not have a valid name.");
            }

            var res = await Client.Orders.GetOrders(exchange.ExchCode);
            var toreturn = res.Result.Select(x => x.ToCommonUserOrderItem()).ToList();
            toreturn.ForEach(x => x.ExchangeRef = this.CryptoExchange);
            return toreturn;
        }
    }
}
