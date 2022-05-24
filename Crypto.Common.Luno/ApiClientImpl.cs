using Crypto.Common.Interfaces;
using Crypto.Common.Luno.Conversion;
using Crypto.Common.Models;
using Crypto.Common.Requests;
using LunoIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Luno
{
    public class ApiClientImpl : IApiClient
    {
        ExchangeImpl _exchange;
        LunoClient Client { get => _exchange?.ActualExchange?.Client; }

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

        public override Task<List<CommonExchange>> GetExchangesAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task<List<CommonMarket>> GetMarketsAsync()
        {
            var res = await Client.GetMarketsInfoAsync();
            var toreturn = res.Markets.Select(x => x.ToCommonMarket()).ToList();
            toreturn.ForEach(x =>
            {
                x.ExchangeRef = this.CryptoExchange;
                x.ExchCode = "Luno";
            });
            return toreturn;
        }

        public override Task<List<CommonMarket>> GetMarketsAsync(CommonExchange exchange)
        {
            throw new NotImplementedException();
        }

        public override async Task<List<CommonUserOrderItem>> GetOrdersAsync()
        {
            var res = await Client.GetOrdersAsync(LunoIntegration.Enums.OrderStates.Pending);
            var toreturn = res.Orders?.Select(x => x.ToCommonUserOrderItem()).ToList();

            if (toreturn != null)
            {
                toreturn.ForEach(x =>
                {
                    x.ExchangeRef = this.CryptoExchange;
                    x.ExchCode = "Luno";
                });
            }
           

            return toreturn;
        }

        public override async Task<List<CommonUserOrderItem>> GetOrdersAsync(CommonExchange exchange)
        {
            return await GetOrdersAsync();
        }
    }
}
