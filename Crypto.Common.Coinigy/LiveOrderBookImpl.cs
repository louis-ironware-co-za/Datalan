using Crypto.Coinigy;
using Crypto.Common.Coinigy.Conversion;
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
    public class LiveOrderBookImpl : CommonLiveOrderBook
    {
        public CoinigyLiveOrderBook ActualLiveOrderBook { get; private set; }
        public LiveOrderBookImpl(CoinigyLiveOrderBook cLiveOrderBook, ICryptoExchange cryptoExchange, CommonExchange exchange, CommonMarket market, CommonApiCredentials credentials):base(cryptoExchange, exchange, market, credentials)
        {
            this.ActualLiveOrderBook = cLiveOrderBook;
        }

        public override void Subscribe()
        {
            this.ActualLiveOrderBook.OrderBookUpdated += ActualLiveOrderBook_OrderBookUpdated;
            this.ActualLiveOrderBook.TradeUpdated += ActualLiveOrderBook_TradeUpdated;
            this.ActualLiveOrderBook.Connect();
        }

        private void ActualLiveOrderBook_TradeUpdated(CoinigyLiveOrderBook book)
        {
            this.TradeItems.Clear();

            foreach (var item in book.Trades)
            {
                var toAdd = item.ToCommonTradeItem();
                toAdd.ExchangeRef = this.CryptoExchange;
                toAdd.MarketRef = this.Market;
                TradeItems.Add(toAdd);
            }

            OnOrderBookTradeUpdated();
        }

        private void ActualLiveOrderBook_OrderBookUpdated(CoinigyLiveOrderBook book)
        {
            this.OrderBook.Buys.Clear();

            foreach (var item in book.OrderBook.Buys)
            {
                var toAdd = item.ToCommonOrderItem();
                toAdd.ExchangeRef = this.CryptoExchange;
                toAdd.MarketRef = this.Market;
                this.OrderBook.Buys.Add(toAdd);
            }

            this.OrderBook.Sells.Clear();

            foreach (var item in book.OrderBook.Sells)
            {
                var toAdd = item.ToCommonOrderItem();
                toAdd.ExchangeRef = this.CryptoExchange;
                toAdd.MarketRef= this.Market;
                this.OrderBook.Sells.Add(toAdd);
            }

            OnOrderBookUpdated();
        }

        public override void Unsubscribe()
        {
            throw new NotImplementedException();
        }
    }
}
