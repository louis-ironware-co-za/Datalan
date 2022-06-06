using Coinigy.API.V2;
using Coinigy.API.V2.Models;
using Coinigy.API.V2.Requests;
using Crypto.Common.Coinigy.Conversion;
using Crypto.Common.Conversion;
using Crypto.Common.Interfaces;
using Crypto.Common.Models;
using Crypto.Common.Requests;
using Crypto.Common.Responses;
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

        public override async Task<bool> CancelMarketOrder(CommonCancelOrderRequest request)
        {
            var res = await Client.Orders.CancelOrder(request.AuthId, request.OrderId);

            if (!res.Success)
            {
                throw new Exception(res.Error);
            }
            return res.Success;
        }

        public override async Task<CommonNewOrderResponse> CreateMarketOrder(CommonNewOrderRequest request)
        {
            if (string.IsNullOrEmpty(request.AuthId))
            {
                throw new ArgumentException("Invalid auth id for this exchange. [" + _exchange.ExchangeName + "]");
            }

            var req = new NewOrderRequest()
            {
                ExchMktId = request.ExchMktId,
                ExpiresAt = request.ExpiresAt,
                Price = request.Price,
                PriceType = request.PriceType,
                Quantity = request.Quantity,
                Side = request.Side,
            };

            var res = await Client.Orders.PlaceNewOrder(req, request.AuthId);

            return new CommonNewOrderResponse()
            {
                ForeignOrderId = res.ForeignOrderId,
                OrderId = res.CoinigyOrderId == null ? null : res.CoinigyOrderId.ToString(),
                Tag = res,
            };
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

        public override async Task<List<CommonAccountBalance>> GetAccountBalancesAsync()
        {
            var allAccounts = await Client.ExchangeAccounts.GetExchangeAccountsAsync();

            List<CommonAccountBalance> commonAccountBalances = new List<CommonAccountBalance>();

            foreach (var acc in allAccounts.Result)
            {
                var accBalance = await Client.ExchangeAccounts.GetBalanceAsync(acc.AuthId.ToString());
                var toAdd = accBalance.Result.Select(x => x.ToCommonAccountBalance()).ToList();
                toAdd.ForEach(x =>
                {
                    x.ExchangeRef = this.CryptoExchange;
                    x.AuthId = acc.AuthId.ToString();
                    x.AuthKey = acc.AuthKey.ToString();
                    x.AuthNickname = acc.AuthNickname;
                });
                commonAccountBalances.AddRange(toAdd);
            }

            return commonAccountBalances;
        }
    }
}
