using LunoIntegration.Enums;
using LunoIntegration.Models;
using LunoIntegration.StreamModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LunoIntegration
{
    public class LunoLiveOrderBook
    {
        #region EventsAnDelegate
        public delegate void OrderBookInitializedDelegate(LunoLiveOrderBook book);
        public event OrderBookInitializedDelegate OrderBookInitialized;
        public delegate void OrderBookUpdatedDelegate(LunoLiveOrderBook book);
        public event OrderBookUpdatedDelegate OrderBookUpdated;
        public delegate void OrderBookStatusChangedDelegate(LunoLiveOrderBook book);
        public event OrderBookStatusChangedDelegate OrderBookStatusChanged;
        public delegate void OrderBookTradeUpdatedDelegate(LunoLiveOrderBook book);
        public event OrderBookTradeUpdatedDelegate OrderBookTradeUpdated;
        #endregion

        public LiveOrderBook OrderBook { get; private set; }
        public List<TradeUpdate> Trades { get; private set; } = new List<TradeUpdate>();
        public MarketInfo Market { get; private set; }
        private ClientWebSocket webSocket;
        private CancellationToken socketCancellationToken = CancellationToken.None;

        string key = "";
        string secret = "";

        private Action<string> orderBookReceivedAction;
        private Action<string> orderUpdateReceivedAction;
        private Action disconnectedAction;

        public LunoLiveOrderBook(MarketInfo market, CancellationToken token, string apiKey, string apiSecret)
        {
            Market = market;
            this.key = apiKey;
            this.secret = apiSecret;
        }

        

        private async void StartListen()
        {
            var buffer = new byte[51200];

            try
            {
                var stringResult = new StringBuilder();

                while (!this.socketCancellationToken.IsCancellationRequested && this.webSocket.State == WebSocketState.Open)
                {
                    var result = await this.webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), this.socketCancellationToken);

                    var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    stringResult.Append(str);

                    if (result.EndOfMessage)
                    {
                        this.ProcessMessage(stringResult);
                    }
                }
            }
            catch (Exception)
            {
                this.Disconnected();
            }
            finally
            {
                this.webSocket.Dispose();
            }
        }

        private async Task SendMessage(string message)
        {
            if (this.webSocket.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }

            var sendBuffer = new ArraySegment<Byte>(Encoding.UTF8.GetBytes(message.ToString()));
            await this.webSocket.SendAsync(sendBuffer, WebSocketMessageType.Text, true, this.socketCancellationToken);
        }

        private async void SendCredentials()
        {
            var credentials = new StreamCredentials
            {
                Key = this.key,
                Secret = this.secret
            };

            await this.SendMessage(JsonConvert.SerializeObject(value: credentials));
        }

        private void ProcessMessage(StringBuilder stringBuilder)
        {
            var finalString = stringBuilder.ToString();

            if (finalString.Contains("asks") || finalString.Contains("bids"))
            {
                this.orderBookReceivedAction?.Invoke(obj: finalString);
            }
            else
            {
                this.orderUpdateReceivedAction?.Invoke(obj: finalString);
            }

            stringBuilder.Clear();
        }

        private void Disconnected()
        {
            this.disconnectedAction?.Invoke();
        }

        public async Task ConnectLive()
        {
            await ConnectLive(new Action<string>(OnOrderBookReceived), new Action<string>(OnOrderUpdateReceived), new Action(OnDisconnected));
        }

        private void OnOrderBookReceived(string data)
        {
            System.Diagnostics.Debug.WriteLine("Book Received: " + data);

            OrderBook = JsonConvert.DeserializeObject<LiveOrderBook>(data);
            OrderBookInitialized?.Invoke(this);
        }

        private async void OnOrderUpdateReceived(string data)
        {
            System.Diagnostics.Debug.WriteLine("Update Received: " + data);

            LiveUpdate update = JsonConvert.DeserializeObject<LiveUpdate>(data);

            if (update == null)
            {
                //TODO: What should happen here? Is this just a ping?
                return;
            }

            if (update.Sequence != OrderBook.Sequence + 1)
            {
                await DisconnectLive();
                throw new Exception("Order book out of sync.");
                //TODO: Raise an out of sync event and maybe try and reestablish a connection.
            }
            
            if (update.CreateUpdate != null)
            {
                switch (update.CreateUpdate.Type)
                {
                    case "BID":
                        OrderBook.Bids.Add(new LiveOrder()
                        {
                            Id = update.CreateUpdate.OrderId,
                            Price = update.CreateUpdate.Price,
                            Volume = update.CreateUpdate.Volume,
                        });
                        break;
                    case "ASK":
                        OrderBook.Asks.Add(new LiveOrder()
                        {
                            Id = update.CreateUpdate.OrderId,
                            Price = update.CreateUpdate.Price,
                            Volume = update.CreateUpdate.Volume,
                        });
                        break;
                    default:
                        break;
                }
            }
            
            if(update.DeleteUpdate != null)
            {
                var match = OrderBook.Asks.FirstOrDefault(x => x.Id == update.DeleteUpdate.OrderId);

                if (match != null)
                {
                    OrderBook.Asks.Remove(match);
                }
                else
                {
                    match = OrderBook.Bids.FirstOrDefault(x => x.Id == update.DeleteUpdate.OrderId);

                    if (match != null)
                    {
                        OrderBook.Bids.Remove(match);
                    }
                }
            }

            bool tradesUpdated = false;
            
            if(update.TradeUpdates != null)
            {
                foreach (var trade in update.TradeUpdates)
                {
                    var match = OrderBook.Asks.FirstOrDefault(x => x.Id == trade.OrderId);
                    //TODO: I imagine if the match is found in BIDS and ASKS we can then assume that the trade is either a buy or sell?
                    if (match != null)
                    {
                        match.Volume -= trade.Base;
                    }
                    else
                    {
                        match = OrderBook.Bids.FirstOrDefault(x => x.Id == trade.OrderId);
                        match.Volume -= trade.Base;
                    }

                    trade.Order = match;
                    Trades.Add(trade);
                    tradesUpdated = true;
                }
            }
            
            if(update.StatusUpdate != null)
            {
                switch (update.StatusUpdate)
                {
                    case "POST_ONLY":
                        OrderBook.Status = TradingStatus.Post_Only;
                        break;
                    case "ACTIVE":
                        OrderBook.Status = TradingStatus.Active;
                        break;
                    case "SUSPENDED":
                        OrderBook.Status = TradingStatus.Suspended;
                        break;
                    default:
                        throw new Exception("Unknown Book Status: [" + update.StatusUpdate + "]");
                }

                OrderBookStatusChanged?.Invoke(this);
                return;
            }
            OrderBook.Sequence = update.Sequence;
            OrderBookUpdated?.Invoke(this);

            if (tradesUpdated)
            {
                OrderBookTradeUpdated?.Invoke(this);
            }

        }

        private void OnDisconnected()
        {
            //TODO: Implement
        }

        public async Task ConnectLive(Action<string> orderBookReceivedAction, Action<string> orderUpdateReceivedAction, Action disconnectedAction)
        {
            this.orderBookReceivedAction = orderBookReceivedAction;
            this.orderUpdateReceivedAction = orderUpdateReceivedAction;
            this.disconnectedAction = disconnectedAction;

            await Task.Factory.StartNew(async () => await this.CreateSocketConnection(pair: Market.MarketId), this.socketCancellationToken);
        }

        private async Task CreateSocketConnection(string pair)
        {
            var uri = new Uri(uriString: $"wss://ws.luno.com/api/1/stream/{pair}");
            this.webSocket = new ClientWebSocket();
            this.webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(10);

            await this.webSocket.ConnectAsync(uri: uri, cancellationToken: this.socketCancellationToken);

            this.SendCredentials();
            this.StartListen();
        }

        public async Task DisconnectLive()
        {
            if (this.webSocket.State == WebSocketState.Open)
            {
                await this.webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, this.socketCancellationToken);
            }
        }
    }
}
