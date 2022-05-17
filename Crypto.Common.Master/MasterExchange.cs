using Crypto.Common.Interfaces;
using Crypto.Common.Models;
using Crypto.Common.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Master
{
    public class MasterExchange : IDisposable
    {
        #region Events
        public event MasterOrderBookUpdatedDelegate OrderBookUpdated;
        #endregion

        public List<ICryptoExchange> CryptoExchanges { get; private set; } = new List<ICryptoExchange>();
        public MasterLiveOrderBook MasterLiveOrderBook { get; private set; }

        private List<CommonExchange> _exchanges = new List<CommonExchange>();
        public List<CommonExchange> Exchanges
        {
            get
            {
                _exchanges.Clear();

                foreach (var cryptoExchange in CryptoExchanges)
                {
                    _exchanges.AddRange(cryptoExchange.Exchanges);
                }

                return _exchanges;
            }
        }

        public MasterExchange()
        {
            MasterLiveOrderBook = new MasterLiveOrderBook(this);
            MasterLiveOrderBook.MasterOrderBookUpdated += MasterLiveOrderBook_MasterOrderBookUpdated;
        }

        private void MasterLiveOrderBook_MasterOrderBookUpdated(MasterLiveOrderBook book)
        {
            this.OrderBookUpdated?.Invoke(book);
        }

        public void AddCryptoExchange(MasterApiCredentials credentials)
        {
            if (credentials == null)
            {
                throw new ArgumentNullException("credentials");
            }

            if (!CryptoExchanges.Any(x => x.Exchange == credentials.Exchange))
            {
                ICryptoExchange added;
                switch (credentials.Exchange)
                {
                    case Common.Exchanges.Coinigy:
                        Coinigy.ExchangeImpl exCoin = new Coinigy.ExchangeImpl(credentials);
                        added = exCoin;
                        CryptoExchanges.Add(exCoin);
                        break;
                    case Common.Exchanges.Luno:
                        Luno.ExchangeImpl exLuno = new Luno.ExchangeImpl(credentials);
                        added = exLuno;
                        CryptoExchanges.Add(exLuno);
                        break;
                    default:
                        throw new Exception("Exchange not supported. [" + credentials.Exchange.ToString() + "]");
                }

                if (added != null)
                {
                    SubscribeToExchangeOrderBookEvents(added);
                }
            }
        }

        public async Task PopulateMarketsForExchanges(List<CommonExchange> exchanges)
        {
            if (exchanges == null)
            {
                throw new ArgumentNullException(nameof(exchanges));
            }

            foreach (var ex in exchanges.GroupBy(x => x.ParentExchange))
            {
                await GetCryptoExchange(ex.Key).PopulateMarkets(ex.ToList());
            }
        }

        public ICryptoExchange GetCryptoExchange(Exchanges exchange)
        {
            var match = CryptoExchanges.FirstOrDefault(x => x.Exchange == exchange);

            if (match == null)
            {
                throw new Exception("The requested exchange has not been added. [" + exchange + "]");
            }

            return match;
        }

        public async Task<List<CommonMarket>> GetAllMarkets()
        {
            List<CommonMarket> toreturn = new List<CommonMarket>();

            foreach (var item in CryptoExchanges)
            {
                toreturn.AddRange(item.Markets);
                //var res = await item.ApiClient.GetMarketsAsync();
                //toreturn.AddRange(res);
            }

            return toreturn;
        }

        private void SubscribeToExchangeOrderBookEvents(ICryptoExchange exchange)
        {
            exchange.OrderBookUpdated += Exchange_OrderBookUpdated;
            exchange.OrderBookInitialized += Exchange_OrderBookInitialized;
            exchange.OrderBookTradeUpdated += Exchange_OrderBookTradeUpdated;
            exchange.OrderBookStatusUpdated += Exchange_OrderBookStatusUpdated;
        }

        private void Exchange_OrderBookStatusUpdated(Models.CommonLiveOrderBook book)
        {

        }

        private void Exchange_OrderBookTradeUpdated(Models.CommonLiveOrderBook book)
        {

        }

        private void Exchange_OrderBookInitialized(Models.CommonLiveOrderBook book)
        {

        }

        private void Exchange_OrderBookUpdated(Models.CommonLiveOrderBook book)
        {

        }

        public async Task InitializeAll()
        {
            foreach (var item in CryptoExchanges)
            {
                await item.Initialize();
            }
        }

        public void SubscribeToMarketOrderBook(ICryptoExchange exchange, CommonMarket market)
        {
            MasterLiveOrderBook.RegisterExchange(exchange);
            exchange.SubscribeToOrderBook(market.ExchCode, market);
        }

        public void DisconnectAll()
        {
            CryptoExchanges.ForEach(x => x.Dispose());
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
        // ~MasterExchange()
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
