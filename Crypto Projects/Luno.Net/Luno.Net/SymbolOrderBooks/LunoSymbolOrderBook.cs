using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.OrderBook;
using CryptoExchange.Net.Sockets;
using Luno.Net.Clients;
using Luno.Net.Interfaces;
using Luno.Net.Interfaces.Clients;
using Luno.Net.Objects;
using Luno.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luno.Net.SymbolOrderBooks
{
    public class LunoSymbolOrderBook : SymbolOrderBook
    {
        private readonly ILunoClient _restClient;
        private readonly ILunoSocketClient _socketClient;
        private readonly bool _restOwner;
        private readonly bool _socketOwner;
        private readonly int? _updateInterval;

        public LunoSymbolOrderBook(string symbol, LunoOrderBookOptions? options = null):base("Luno", symbol, options ?? new LunoOrderBookOptions())
        {
            //TODO: validate symbol;
            Levels = options?.Limit;
            _updateInterval = options?.UpdateInterval;
            _socketClient = options?.SocketClient ?? new LunoSocketClient();
            _restClient = options?.RestClient ?? new LunoClient();
            _restOwner = options?.RestClient == null;
            _socketOwner = options?.SocketClient == null;

            sequencesAreConsecutive = options?.Limit == null;
            strictLevels = false;

            this.OnOrderBookUpdate += LunoSymbolOrderBook_OnOrderBookUpdate;
            _socketClient.OnSubscriptionDataReceived += _socketClient_OnSubscriptionDataReceived;


        }

        private void _socketClient_OnSubscriptionDataReceived(ILunoSocketClient client, CallResult<LunoEventOrderBook> data)
        {
            if (data.Data is LunoEventOrderBook book)
            {
                HandleUpdateInternal(book);
            }
        }

        private void LunoSymbolOrderBook_OnOrderBookUpdate((IEnumerable<CryptoExchange.Net.Interfaces.ISymbolOrderBookEntry> Bids, IEnumerable<CryptoExchange.Net.Interfaces.ISymbolOrderBookEntry> Asks) obj)
        {

        }

        protected override async Task<CallResult<UpdateSubscription>> DoStartAsync(CancellationToken ct)
        {
            CallResult<UpdateSubscription> subResult = null;
            if(Levels == null)
                subResult = await _socketClient.GeneralStreams.SubscribeToMarketStreamAsync(Symbol, "TEST", "TEST", HandleUpdate).ConfigureAwait(false);
            
            if (subResult == null || !subResult)
            {
                return new CallResult<UpdateSubscription>(subResult?.Error);
            }

            if (ct.IsCancellationRequested)
            {
                await subResult.Data.CloseAsync().ConfigureAwait(false);
                return subResult.AsError<UpdateSubscription>(new CancellationRequestedError());
            }

            Status = OrderBookStatus.Syncing;
            var setResult = await WaitForSetOrderBookAsync(30000, ct).ConfigureAwait(false);
            return setResult ? subResult : subResult = new CallResult<UpdateSubscription>(setResult.Error);
            //First result should be order book snapshot.
            //return new CallResult<UpdateSubscription>(subResult.Data);

        }

        private void HandleUpdate(DataEvent<LunoEventOrderBook> data)
        {
            HandleUpdateInternal(data.Data);
        }

        private void HandleUpdateInternal(LunoEventOrderBook data)
        {
            if (data == null)
                return;
            if (data.Asks != null && data.Bids != null)
            {
                if (this.Status == OrderBookStatus.Connecting || this.Status == OrderBookStatus.Reconnecting)
                {
                    SetInitialOrderBook(data.Sequence, data.Bids, data.Asks);
                }
            }
            else
            {
                if (data.Status_Update != null)
                {
                    
                }
                else if(data.Delete_Update != null)
                {
                    //UpdateOrderBook()
                }
            }
        }

        protected override bool ProcessUpdate(long sequence, OrderBookEntryType type, ISymbolOrderBookEntry entry)
        {
            return base.ProcessUpdate(sequence, type, entry);
        }

        protected override async Task<CallResult<bool>> DoResyncAsync(CancellationToken ct)
        {
            //if (Levels != null)
            //    return await WaitForSetOrderBookAsync(10000, ct).ConfigureAwait(false);

            //var bookResult = await _socketClient.subsc
            //if (!bookResult)
            //    return new CallResult<bool>(bookResult.Error!);

            //SetInitialOrderBook(bookResult.Data.LastUpdateId, bookResult.Data.Bids, bookResult.Data.Asks);
            return new CallResult<bool>(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (_restOwner)
            {
                _restClient.Dispose();
            }
            if (_socketOwner)
            {
                _socketClient.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
