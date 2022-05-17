using Crypto.Common.Models;
using Crypto.Common.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Interfaces
{
    public abstract class ICryptoExchange : IDisposable
    {
        #region Events
        public event OrderBookInitializedDelegate OrderBookInitialized;
        public event OrderBookUpdatedDelegate OrderBookUpdated;
        public event OrderBookTradeUpdatedDelegate OrderBookTradeUpdated;
        public event OrderBookStatusUpdatedDelegate OrderBookStatusUpdated;
        #endregion
        public Exchanges Exchange { get; private set; } 
        public string ExchangeName { get => Exchange.ToString(); }
        public CommonApiCredentials Credentials { get; private set; }
        public IApiClient ApiClient { get; set; }

        public List<CommonLiveOrderBook> LiveOrderBooks { get; private set; } = new List<CommonLiveOrderBook>();
        public List<CommonExchange> Exchanges { get; private set; } = new List<CommonExchange>();

        public List<CommonMarket> Markets { get; private set; } = new List<CommonMarket>();

        public bool IsMultiExchange { get => Exchanges.Count > 1; }

        public ICryptoExchange(Exchanges exchange, CommonApiCredentials credentials)
        {
            this.Exchange = exchange;
            Credentials = credentials;
        }

        public abstract Task Initialize();

        protected abstract Task PopulateExchanges();

        public abstract Task PopulateMarkets(List<CommonExchange> exchanges);
        public void ClearMarkets()
        {
            Markets.Clear();
        }

        public abstract CommonLiveOrderBook SubscribeToOrderBook(CommonExchange exchange, CommonMarket market);

        public abstract CommonLiveOrderBook SubscribeToOrderBook(string exchCode, CommonMarket market);

        protected void OnOrderBookUpdated(CommonLiveOrderBook book)
        {
            OrderBookUpdated?.Invoke(book);
        }

        protected void OnOrderBookInitialized(CommonLiveOrderBook book)
        {
            OrderBookInitialized?.Invoke(book);
        }

        protected void OnOrderBookTradeUpdated(CommonLiveOrderBook book)
        {
            OrderBookTradeUpdated?.Invoke(book);
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
                LiveOrderBooks.ForEach(x => x.Dispose());
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ICryptoExchange()
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
