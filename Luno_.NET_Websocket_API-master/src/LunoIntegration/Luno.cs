using LunoIntegration;
using LunoIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LunoIntegration
{
    public class Luno : IDisposable
    {
        private bool disposedValue;

        private string ApiKey { get; set; } = "";
        private string ApiSecret { get; set; } = "";
        public LunoClient Client { get; private set; }

        CancellationTokenSource SubscriptionCancellationToken = new CancellationTokenSource();

        List<LunoLiveOrderBook> LunoLiveOrderBooks = new List<LunoLiveOrderBook>();

        public Luno(string key, string secret)
        {
            ApiKey = key;
            ApiSecret = secret;
        }

        public void Initialize()
        {
            Client = new LunoClient(this.ApiKey, this.ApiSecret);
        }

        public async Task<MarketsInfo> GetMarketsAsync()
        {
            return await Client.GetMarketsInfoAsync();
        }

        public async Task<LunoLiveOrderBook> SubscribeToOrderBook(MarketInfo market)
        {
            if (!LunoLiveOrderBooks.Any(x => x.Market.MarketId == market.MarketId))
            {
                var newBook = new LunoLiveOrderBook(market, SubscriptionCancellationToken.Token, ApiKey, ApiSecret);
                LunoLiveOrderBooks.Add(newBook);
                await newBook.ConnectLive();
                return newBook;
                //await Client.ConnectLive(market.MarketId, new Action<string>(OnOrderBookReceived), new Action<string>(OnOrderUpdateReceived), new Action(OnDisconnected), SubscriptionCancellationToken.Token);
            }

            return null;
        }

        public async Task DisconnectAll()
        {
            foreach (var item in LunoLiveOrderBooks)
            {
                await item.DisconnectLive();
            }
        }

        #region Dispose
        protected async virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                await DisconnectAll();

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Luno()
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
