using Crypto.Common.Interfaces;
using Crypto.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Master
{
    public class MasterLiveOrderBook
    {
        #region Events
        public event MasterOrderBookUpdatedDelegate MasterOrderBookUpdated;
        #endregion

        public List<CommonLiveOrderBook> OrderBooks
        {
            get
            {
                return CryptoExchanges.SelectMany(x => x.LiveOrderBooks).ToList();
            }
        }
        public List<ICryptoExchange> CryptoExchanges { get; private set; } = new List<ICryptoExchange>();

        private MasterExchange MasterExchange { get; set; }

        private List<CommonOrderItem> _buys = new List<CommonOrderItem>();
        public List<CommonOrderItem> Buys
        {
            get
            {
                return _buys;
            }
        }

        private List<CommonOrderItem> _sells = new List<CommonOrderItem>();
        public List<CommonOrderItem> Sells
        {
            get
            {
                return _sells;
            }
        }

        private List<CommonTradeItem> _trades = new List<CommonTradeItem>();
        public List<CommonTradeItem> Trades
        {
            get
            {
                return _trades;
            }
        }


        public MasterLiveOrderBook(MasterExchange masterExchange)
        {
            this.MasterExchange = masterExchange;
        }

        public void RegisterExchange(ICryptoExchange exchange)
        {
            if (exchange == null)
            {
                throw new ArgumentNullException(nameof(exchange));
            }

            var match = CryptoExchanges.FirstOrDefault(x => x == exchange);

            if (match == null)
            {
                CryptoExchanges.Add(exchange);
                exchange.OrderBookInitialized += Exchange_OrderBookInitialized;
                exchange.OrderBookTradeUpdated += Exchange_OrderBookTradeUpdated;
                exchange.OrderBookUpdated += Exchange_OrderBookUpdated;
            }
        }

        private void Exchange_OrderBookUpdated(CommonLiveOrderBook book)
        {
            System.Diagnostics.Debug.WriteLine("Exchange Order Book Updated.");
            UpdateOrderBook();
        }

        private void UpdateOrderBook()
        {
            Stopwatch sw = Stopwatch.StartNew();
            _buys.Clear();
            _buys.AddRange(OrderBooks.SelectMany(x => x.OrderBook.Buys));

            _sells.Clear();
            _sells.AddRange(OrderBooks.SelectMany(_ => _.OrderBook.Sells));

            _trades.Clear();
            _trades.AddRange(OrderBooks.SelectMany(x => x.TradeItems));

            sw.Stop();
            System.Diagnostics.Debug.WriteLine("Master Book Update: " + sw.Elapsed);
            MasterOrderBookUpdated?.Invoke(this);
        }

        private void Exchange_OrderBookTradeUpdated(CommonLiveOrderBook book)
        {
            UpdateOrderBook();
        }

        private void Exchange_OrderBookInitialized(CommonLiveOrderBook book)
        {
            UpdateOrderBook();
        }
    }
}
