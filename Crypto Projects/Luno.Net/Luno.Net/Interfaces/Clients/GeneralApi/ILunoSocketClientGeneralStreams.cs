using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Luno.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luno.Net.Interfaces.Clients.GeneralApi
{
    public interface ILunoSocketClientGeneralStreams : IDisposable
    {
        Task<CallResult<UpdateSubscription>> SubscribeToMarketStreamAsync(string symbol, string apiKey, string apiSecret, Action<DataEvent<LunoEventOrderBook>> onMessage, CancellationToken ct = default);
    }
}

