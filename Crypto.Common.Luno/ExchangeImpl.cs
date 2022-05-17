using Crypto.Common.Interfaces;
using Crypto.Common.Luno.Conversion;
using Crypto.Common.Models;
using Crypto.Common.Objects;
using LunoIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Luno
{
    public class ExchangeImpl : ICryptoExchange
    {
        public LunoIntegration.Luno ActualExchange { get; private set; }
        public CommonApiCredentials ApiCredentials { get; private set; }

        public ExchangeImpl(CommonApiCredentials credentials):base(Common.Exchanges.Luno, credentials)
        {
            ActualExchange = new LunoIntegration.Luno(credentials.ApiKey, credentials.ApiSecret);
            ApiClient = new ApiClientImpl(this);
        }
        public override async Task Initialize()
        {
            ActualExchange.Initialize();
            await PopulateExchanges();
            //await PopulateMarkets();
        }

        public override CommonLiveOrderBook SubscribeToOrderBook(CommonExchange exchange, CommonMarket market)
        {
            //TODO: Let exchange implement a cancellation token.
            LunoLiveOrderBook book = new LunoLiveOrderBook(market.ToMarketInfo(), System.Threading.CancellationToken.None, Credentials.ApiKey, Credentials.ApiSecret);
            LiveOrderBookImpl impl = new LiveOrderBookImpl(book, this, null, market, Credentials);
            impl.OrderBookUpdated += Impl_OrderBookUpdated;
            impl.OrderBookTradeUpdated += Impl_OrderBookTradeUpdated;
            impl.OrderBookStatusUpdated += Impl_OrderBookStatusUpdated;
            this.LiveOrderBooks.Add(impl);
            impl.Subscribe();
            return impl;
        }
        public override CommonLiveOrderBook SubscribeToOrderBook(string exchCode, CommonMarket market)
        {
            //TODO: Let exchange implement a cancellation token.
            LunoLiveOrderBook book = new LunoLiveOrderBook(market.ToMarketInfo(), System.Threading.CancellationToken.None, Credentials.ApiKey, Credentials.ApiSecret);
            LiveOrderBookImpl impl = new LiveOrderBookImpl(book, this, null, market, Credentials);
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

        protected override async Task PopulateExchanges()
        {
            this.Exchanges.Add(new CommonExchange()
            {
                ExchCode = "Luno",
                ExchEnabled = true,
                ExchTradeEnabled = true,
                //TODO: Is there an API call for fees or do we just get it from the website as static values.
                ExchName = "Luno",
                ParentExchange = this.Exchange,
            });
        }

        public override async Task PopulateMarkets(List<CommonExchange> exchanges)
        {
            if (!exchanges.All(x => x.ParentExchange == this.Exchange))
            {
                throw new InvalidOperationException("One or more exchanges are invalid for this crypto exchange.");
            }

            var markets = await ApiClient.GetMarketsAsync();

            this.Markets.Clear();

            foreach (var item in markets)
            {
                this.Markets.Add(item);
            }
        }

        
    }
}
