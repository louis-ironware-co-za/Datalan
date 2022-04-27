using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Luno.Net.Clients;
using Luno.Net.Objects.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.SymbolOrderBooks
{
    public class CustomLunoSymbolOrderBook :IDisposable
    {
        public event Action<OrderBookStatus, OrderBookStatus>? OnStatusChange;
        public event Action<(IEnumerable<LunoOrderBookEntry> Bids, IEnumerable<LunoOrderBookEntry> Asks)>? OnOrderBookUpdate;

        /// <summary>
        /// The bid list, should only be accessed using the bookLock
        /// </summary>
        protected Dictionary<string, LunoOrderBookEntry> bids;
        public IEnumerable<LunoOrderBookEntry> Bids { get; set; }

        /// <summary>
        /// The ask list, should only be accessed using the bookLock
        /// </summary>
        protected Dictionary<string, LunoOrderBookEntry> asks;
        public IEnumerable<LunoOrderBookEntry> Asks { get; set; }

        private LunoClient _restClient;
        private LunoSocketClient _socketClient;

        Log log;

        bool BookSet;

        /// <summary>
        /// Whether update numbers are consecutive. If set to true and an update comes in which isn't the previous sequences number + 1
        /// the book will resynchronize as it is deemed out of sync
        /// </summary>
        protected bool sequencesAreConsecutive;

        private OrderBookStatus _status;
        private bool disposedValue;

        public OrderBookStatus Status
        {
            get => _status;
            set
            {
                if (value == _status)
                    return;

                var old = _status;
                _status = value;
                //log.Write(LogLevel.Information, $"{Id} order book {Symbol} status changed: {old} => {value}");
                OnStatusChange?.Invoke(old, _status);
            }
        }

        public string Symbol { get; private set; }

        public DateTime UpdateTime { get; private set; }

        public int AskCount { get; private set; }
        public int BidCount { get; private set; }

        public CustomLunoSymbolOrderBook()
        {

        }

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
        // ~CustomLunoSymbolOrderBook()
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
    }
}
