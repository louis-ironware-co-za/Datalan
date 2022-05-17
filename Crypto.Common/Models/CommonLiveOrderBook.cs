using Crypto.Common.Interfaces;
using Crypto.Common.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Models
{
    public abstract class CommonLiveOrderBook : IDisposable
    {
        #region Events
        public event OrderBookInitializedDelegate OrderBookInitialized;
        public event OrderBookUpdatedDelegate OrderBookUpdated;
        public event OrderBookTradeUpdatedDelegate OrderBookTradeUpdated;
        public event OrderBookStatusUpdatedDelegate OrderBookStatusUpdated;
        #endregion

        public CommonOrderBook OrderBook { get; private set; }
        public List<CommonTradeItem> TradeItems { get; private set; } = new List<CommonTradeItem>();
        public CommonMarket Market { get; private set;}
        public CommonExchange Exchange { get; private set; }
        public ICryptoExchange CryptoExchange { get; private set; }

        public CommonApiCredentials Credentials { get; private set; }

        public CommonLiveOrderBook(ICryptoExchange cryptoExchange, CommonExchange exchange, CommonMarket market, CommonApiCredentials credentials)
        {
            this.CryptoExchange = cryptoExchange;
            this.Exchange = exchange;
            this.Market = market;
            this.Credentials = credentials;

            this.OrderBook = new CommonOrderBook(this.CryptoExchange);
        }

        public abstract void Subscribe();

        public abstract void Unsubscribe();

        protected void OnOrderBookUpdated()
        {
            this.OrderBookUpdated?.Invoke(this);
        }

        protected void OnOrderBookInitialized()
        {
            this.OrderBookInitialized?.Invoke(this);
        }

        protected void OnOrderBookTradeUpdated()
        {
            this.OrderBookTradeUpdated?.Invoke(this);
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
                Unsubscribe();

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CommonLiveOrderBook()
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
