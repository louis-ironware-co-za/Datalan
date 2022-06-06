using Crypto.Common.Interfaces;
using Crypto.Common.Luno.Conversion;
using Crypto.Common.Models;
using Crypto.Common.Requests;
using Crypto.Common.Responses;
using LunoIntegration;
using LunoIntegration.Enums;
using LunoIntegration.Models;
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

        public override async Task<bool> CancelMarketOrder(CommonCancelOrderRequest request)
        {
            var res = await Client.StopOrderAsync(request.OrderId);
            return res.Success;
        }

        public override async Task<CommonNewOrderResponse> CreateMarketOrder(CommonNewOrderRequest request)
        {
            var req = new LimitOrderCriteria()
            {
                BaseAccountId = request.BaseAccountId.ToString(),
                CounterAccountId = request.CounterAccountId.ToString(),
                Pair = request.Pair,
                Price = request.Price,
                Type = (OrderTypes)Enum.Parse(typeof(OrderTypes), request.Type.ToString(), true),
                PostOnly = request.Post_Only,
                Volume = request.Quantity,
            };

            var res = await Client.PostLimitOrderAsync(req);

            var resp = new CommonNewOrderResponse()
            {
                ForeignOrderId = null,
                OrderId = res.OrderId,
                Tag = res,
            };

            return resp;
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

        public override async Task<List<CommonAccountBalance>> GetAccountBalancesAsync()
        {
            var res = await Client.GetAccountBalancesAsync();

            var toreturn = res.Balances.Select(x => x.ToCommonAccountBalance()).ToList();

            toreturn.ForEach(x =>
            {
                x.ExchangeRef = this.CryptoExchange;
            });

            return toreturn;
        }
    }
}
