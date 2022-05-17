using Crypto.Coinigy;
using Crypto.Common.Coinigy.Conversion;
using Crypto.Common.Conversion;
using Crypto.Common.Interfaces;
using Crypto.Common.Models;
using Crypto.Common.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Coinigy
{
    public class ExchangeImpl : ICryptoExchange
    {
        public Crypto.Coinigy.Coinigy ActualExchange { get; private set; }
        public ExchangeImpl(CommonApiCredentials credentials):base(Common.Exchanges.Coinigy, credentials)
        {
            ActualExchange = new Crypto.Coinigy.Coinigy(CommonApiCredentialsExtensions.Convert(credentials));
            ApiClient = new ApiClientImpl(this);
        }

        public override async Task Initialize()
        {
            ActualExchange.Initialize();
            await PopulateExchanges();
            //await PopulateMarkets();
            //var t = Task.Run(async () =>
            //{
            //    ActualExchange.Initialize();
            //    await PopulateExchanges();
            //    await PopulateMarkets();
            //});

            //await t;
        }

        public override CommonLiveOrderBook SubscribeToOrderBook(CommonExchange exchange, CommonMarket market)
        {
            CoinigyLiveOrderBook book = ActualExchange.Subscribe(exchange.ToExchange(), market.ToMarketListEntry());
            LiveOrderBookImpl impl = new LiveOrderBookImpl(book, this, exchange, market, Credentials);
            impl.OrderBookUpdated += Impl_OrderBookUpdated;
            impl.OrderBookTradeUpdated += Impl_OrderBookTradeUpdated;
            impl.OrderBookStatusUpdated += Impl_OrderBookStatusUpdated;
            this.LiveOrderBooks.Add(impl);
            impl.Subscribe();
            return impl;
        }

        private void Impl_OrderBookStatusUpdated(CommonLiveOrderBook book)
        {
            throw new NotImplementedException();
        }

        private void Impl_OrderBookTradeUpdated(CommonLiveOrderBook book)
        {
            OnOrderBookTradeUpdated(book);
        }

        private void Impl_OrderBookUpdated(CommonLiveOrderBook book)
        {
            OnOrderBookUpdated(book);
        }

        public override CommonLiveOrderBook SubscribeToOrderBook(string exchCode, CommonMarket market)
        {
            CommonExchange ex = new CommonExchange() { ExchCode = exchCode };
            CoinigyLiveOrderBook book = ActualExchange.Subscribe(ex.ToExchange(), market.ToMarketListEntry());
            LiveOrderBookImpl impl = new LiveOrderBookImpl(book, this, ex, market, Credentials);
            impl.OrderBookUpdated += Impl_OrderBookUpdated;
            impl.OrderBookTradeUpdated += Impl_OrderBookTradeUpdated;
            impl.OrderBookStatusUpdated += Impl_OrderBookStatusUpdated;
            this.LiveOrderBooks.Add(impl);
            impl.Subscribe();
            return impl;
        }

        protected override async Task PopulateExchanges()
        {
            //TODO: Implement.
            var exchanges = await this.ApiClient.GetExchangesAsync();

            foreach (var item in exchanges)
            {
                this.Exchanges.Add(item);
            }
        }

        public override async Task PopulateMarkets(List<CommonExchange> exchanges)
        {
            this.Markets.Clear();

            foreach (var item in exchanges)
            {
                var markets = await ApiClient.GetMarketsAsync(item);

                foreach (var m in markets)
                {
                    this.Markets.Add(m);
                }
            }
        }
    }
}
