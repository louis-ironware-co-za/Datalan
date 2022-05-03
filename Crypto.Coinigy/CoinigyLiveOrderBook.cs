using Coinigy.API;
using Coinigy.API.V2.Models;
using CoinigyWebsocketClient;
using CoinigyWebsocketClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Crypto.Coinigy
{
    public class CoinigyLiveOrderBook
    {
        #region Events
        public event OrderBookUpdatedDelegate OrderBookUpdated;
        public event TradeUpdatedDelegate TradeUpdated;
        #endregion

        public Exchange Exchange { get; private set; }
        CoinigyWsClient wsClient;
        public string Market { get; private set; }
        public string Curr1 { get; private set; }
        public string Curr2 { get; private set; }

        public LiveOrderBook OrderBook { get; private set; } = new LiveOrderBook();
        public List<TradeItem> Trades { get; private set; } = new List<TradeItem>();

        public CoinigyLiveOrderBook(Exchange exchange, string curr1, string curr2, CoinigyWsClient wsClient)
        {
            Exchange = exchange;
            this.wsClient = wsClient;
            this.Market = curr1 + curr2;
            this.Curr1 = curr1;
            this.Curr2 = curr2;
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            wsClient.OnTradeMessage += WsClient_OnTradeMessage;
            wsClient.OnClientReady += WsClient_OnClientReady;
            wsClient.OnOrderMessage += WsClient_OnOrderMessage;
        }

        public bool Connect()
        {
            var res = wsClient.SubscribeToOrderChannel(Exchange.ExchCode, Curr1, Curr2);
            wsClient.SubscribeToTradeChannel(Exchange.ExchCode, Curr1, Curr2);
            return res;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        private void WsClient_OnOrderMessage(string exchange, string curr1, string curr2, CoinigyWebsocketClient.Models.OrderItem[] orders)
        {
            //Apparently Coinigy sends through a complete order book with each update which means there aren't any syncing issue.
            //Just completely update the local copy.

            if (Exchange.ExchCode == exchange && Market == (curr1 + curr2))
            {
                OrderBook.Sells.Clear();
                orders.Where(x => x.Ordertype.ToLower() == "sell").ToList().ForEach(x => OrderBook.Sells.Add(x));
                OrderBook.Buys.Clear();
                orders.Where(x => x.Ordertype.ToLower() == "buy").ToList().ForEach(x => OrderBook.Buys.Add(x));
                OrderBookUpdated?.Invoke(this);
            }
        }

        private void WsClient_OnClientReady()
        {

        }

        private void WsClient_OnTradeMessage(string exchange, string curr1, string curr2, CoinigyWebsocketClient.Models.TradeItem trade)
        {
            if (Exchange.ExchCode == exchange && Market == (curr1 + curr2))
            {
                Trades.Add(trade);
                TradeUpdated?.Invoke(this);
            }
        }
    }
}
