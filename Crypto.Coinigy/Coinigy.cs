using Coinigy.API;
using Coinigy.API.V2;
using Coinigy.API.V2.Models;
using CoinigyWebsocketClient;
using CoinigyWebsocketClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Coinigy
{
    public class Coinigy : IDisposable
    {
        public event OrderBookUpdatedDelegate OrderBookUpdated;
        public event TradeUpdatedDelegate TradeUpdated;

        ApiCredentials credentials;
        CoinigyWsClient wsClient;
        CoinigyWebSocketClientOptions SocketOptions;
        public APIClient Client { get; private set; }

        List<CoinigyLiveOrderBook> CoinigyLiveOrderBooks = new List<CoinigyLiveOrderBook>();

        public Coinigy()
        {
            
        }

        public Coinigy(string key, string secret) : this()
        {
            this.credentials = new ApiCredentials()
            {
                ApiKey = key,
                SocketKey = key,
                ApiSecret = secret,
                SocketSecret = secret,
            };
        }

        public Coinigy(ApiCredentials credentials) : this()
        {
            this.credentials = credentials;
        }

        public void Initialize()
        {
            SocketOptions = new CoinigyWebSocketClientOptions()
            {
                //MyReconnectStrategy = new PureWebSockets.ReconnectStrategy(10),
#if DEBUG
                DebugMode = true,
#endif
                Creds = new PureSocketCluster.Creds() { apiKey = credentials.SocketKey, apiSecret = credentials.SocketSecret },

            };


            Client = new APIClient(credentials.ApiKey, credentials.ApiSecret);
        }

        private void SetupSocketClient()
        {
            if (wsClient == null)
            {
                wsClient = new CoinigyWsClient(SocketOptions);

                wsClient.OnClosed += WsClient_OnClosed;
                wsClient.OnError += WsClient_OnError;
                wsClient.OnFatality += WsClient_OnFatality;
                wsClient.OnFavoriteMessage += WsClient_OnFavoriteMessage;
                wsClient.OnMessage += WsClient_OnMessage;
                wsClient.OnStateChanged += WsClient_OnStateChanged;
                wsClient.OnSendFailed += WsClient_OnSendFailed;
                wsClient.OnOrderMessage += WsClient_OnOrderMessage;

                wsClient.Connect();
                bool clientReady = false;
                wsClient.OnClientReady += (() =>
                {
                    //wsClient.SubscribeToChannel("TRADE-BMEX--XBT--USD");
                    Console.WriteLine("Client Ready...");
                    clientReady = true;
                });

                while (!clientReady)
                {
                    System.Threading.Thread.Sleep(50);
                }
            }
        }

        private void WsClient_OnOrderMessage(string exchange, string curr1, string curr2, OrderItem[] orders)
        {
            Console.WriteLine("Order Data received.");
        }

        private void WsClient_OnSendFailed(object sender, string data, Exception ex)
        {

        }

        private void WsClient_OnStateChanged(object sender, System.Net.WebSockets.WebSocketState newState, System.Net.WebSockets.WebSocketState prevState)
        {

        }

        private void WsClient_OnMessage(CoinigyWebsocketClient.Types.MessageType messageType, string data)
        {
            
        }

        private void WsClient_OnFavoriteMessage(FavoriteDataItem[] favorites)
        {

        }

        private void WsClient_OnFatality(object sender, string reason)
        {
            Console.WriteLine("Fatality: " + reason);
        }

        private void WsClient_OnError(object sender, Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        private void WsClient_OnClosed(object sender, System.Net.WebSockets.WebSocketCloseStatus reason)
        {
            Console.WriteLine(reason);
        }

        public List<CoinigyLiveOrderBook> Pending = new List<CoinigyLiveOrderBook>();

        private void WsClient_OnClientReady()
        {
            Console.WriteLine("Client Ready...");
        }

        public CoinigyLiveOrderBook Subscribe(Exchange exchange, MarketListEntry market)
        {
            SetupSocketClient();
            //if (!wsClient.Connect())
            //{
            //    throw new Exception("Socket: Failed to connect");
            //}

            var match = CoinigyLiveOrderBooks.FirstOrDefault(x => x.Exchange.ExchCode == exchange.ExchCode && x.Market == market.MarketName);
            if (match == null)
            {
                var cp = (CurrencyPair)market;
                CoinigyLiveOrderBook newBook = new CoinigyLiveOrderBook(exchange, cp.Currency1, cp.Currency2, wsClient);
                newBook.TradeUpdated += NewBook_TradeUpdated;
                newBook.OrderBookUpdated += NewBook_OrderBookUpdated;
                var res = newBook.Connect();

                if (!res)
                {
                    //Disconnect from events
                    newBook.TradeUpdated -= NewBook_TradeUpdated;
                    newBook.OrderBookUpdated -= NewBook_OrderBookUpdated;
                    throw new Exception("Failed to connect");
                }

                CoinigyLiveOrderBooks.Add(newBook);
                match = newBook;
            }

            //probably return a match.
            return match;
        }

        public void Subscribe(string method, string exchCode, string curr1, string curr2)
        {
            SetupSocketClient();
            return;
            string channelName = $"{method}-{exchCode}--{curr1}=={curr2}";
            wsClient.SubscribeToChannel(channelName);

        }

        private void NewBook_OrderBookUpdated(CoinigyLiveOrderBook book)
        {
            this.OrderBookUpdated?.Invoke(book);
        }

        private void NewBook_TradeUpdated(CoinigyLiveOrderBook book)
        {
            this.TradeUpdated?.Invoke(book);
        }

        public void DisconnectAll()
        {
            //TODO: There should be a way to disconnect.
        }



        #region Dispose
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CoinigyLiveOrderBook()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
