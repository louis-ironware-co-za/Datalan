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
    public class LiveOrderBookImpl : CommonLiveOrderBook
    {
        public LunoLiveOrderBook ActualLiveOrderBook { get; private set; }

        public LiveOrderBookImpl(LunoLiveOrderBook lLiveOrderBook, ICryptoExchange cryptoExchange, CommonExchange exchange, CommonMarket market, CommonApiCredentials credentials) : base(cryptoExchange, exchange, market, credentials)
        {
            this.ActualLiveOrderBook = lLiveOrderBook;
        }

        public override void Subscribe()
        {
            this.ActualLiveOrderBook.OrderBookUpdated += ActualLiveOrderBook_OrderBookUpdated;
            this.ActualLiveOrderBook.OrderBookInitialized += ActualLiveOrderBook_OrderBookInitialized;
            this.ActualLiveOrderBook.OrderBookTradeUpdated += ActualLiveOrderBook_OrderBookTradeUpdated;
            _ = this.ActualLiveOrderBook.ConnectLive();
        }

        private void ActualLiveOrderBook_OrderBookTradeUpdated(LunoLiveOrderBook book)
        {
            this.TradeItems.Clear();
            foreach (var item in book.Trades)
            {
                var toAdd = item.ToCommonTradeItem();
                toAdd.MarketRef = this.Market;
                toAdd.ExchangeRef = this.CryptoExchange;
                this.TradeItems.Add(toAdd);
            }

            OnOrderBookTradeUpdated();
        }

        private void ActualLiveOrderBook_OrderBookInitialized(LunoLiveOrderBook book)
        {
            ActualLiveOrderBook_OrderBookUpdated(book);
        }

        private void ActualLiveOrderBook_OrderBookUpdated(LunoLiveOrderBook book)
        {
            this.OrderBook.Buys.Clear();

            foreach (var item in book.OrderBook.Bids)
            {
                var toAdd = item.ToCommonOrderItem();
                toAdd.MarketRef = this.Market;
                toAdd.ExchangeRef = this.CryptoExchange;
                toAdd.OrderType = "Buy";
                this.OrderBook.Buys.Add(toAdd);
            }

            this.OrderBook.Sells.Clear();

            foreach (var item in book.OrderBook.Asks)
            {
                var toAdd = item.ToCommonOrderItem();
                toAdd.MarketRef= this.Market;
                toAdd.ExchangeRef = this.CryptoExchange;
                toAdd.OrderType = "Sell";
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
