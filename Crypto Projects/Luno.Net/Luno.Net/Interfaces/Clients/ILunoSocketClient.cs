using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using Luno.Net.Interfaces.Clients.GeneralApi;
using Luno.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Interfaces.Clients
{
    public delegate void SubscriptionDataRecieved(ILunoSocketClient client, CallResult<LunoEventOrderBook> data);
    public interface ILunoSocketClient : ISocketClient
    {
        event SubscriptionDataRecieved OnSubscriptionDataReceived;
        /// <summary>
        /// Market Stream
        /// </summary>
        ILunoSocketClientGeneralStreams GeneralStreams { get; }
    }
}
