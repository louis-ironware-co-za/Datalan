using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Luno.Net.Clients.GeneralApi;
using Luno.Net.Interfaces.Clients;
using Luno.Net.Interfaces.Clients.GeneralApi;
using Luno.Net.Objects;
using Luno.Net.Objects.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luno.Net.Clients
{
    public class LunoSocketClient : BaseSocketClient, ILunoSocketClient
    {
        public ILunoSocketClientGeneralStreams GeneralStreams { get; }

        public event SubscriptionDataRecieved OnSubscriptionDataReceived;

        public LunoSocketClient() : this(LunoSocketClientOptions.Default)
        {

        }

        public LunoSocketClient(LunoSocketClientOptions options):base("Luno", options)
        {
            SetDataInterpreter((data) => String.Empty, null);
            RateLimitPerSocketPerSecond = 3;
            GeneralStreams = AddApiClient(new LunoSocketClientGeneralStreams(log, this, options));
        }

        public static void SetDefaultOptions(LunoSocketClientOptions options)
        {
            LunoSocketClientOptions.Default = options;
        }

        internal CallResult<T> DeserializeInternal<T>(JToken obj, JsonSerializer? serializer = null, int? requestId = null)
        {
            return Deserialize<T>(obj, serializer, requestId);
        }

        //internal Task<CallResult<UpdateSubscription>> SubscribeInternal<T>(SocketApiClient apiClient, string url, IEnumerable<string> topics, Action<DataEvent<T>> onData, CancellationToken ct)
        //{
        //    var request = new LunoSocketRequest
        //    {
        //        Method = "SUBSCRIBE",
        //        Params = topics.ToArray(),
        //        Id = NextId()
        //    };

        //    return SubscribeAsync(apiClient, url.AppendPath("stream"), request, null, false, onData, ct);
        //}

        internal Task<CallResult<UpdateSubscription>> SubscribeInternal<T>(SocketApiClient apiClient, string url, string topic, string apiKey, string apiSecret, Action<DataEvent<T>> onData, CancellationToken ct)
        {
            var request = new LunoSocketRequest
            {
                ApiKey = apiKey ?? "sqavxb2baxyu",//this.ClientOptions.ApiCredentials.Key,
                ApiSecret = apiSecret ?? "hBZznvdRNNqBwZdbtC6EYpS2GmnVgxBDgGBdBH9wOGM",// this.ClientOptions.ApiCredentials.Secret,
            };
            url = url.AppendPath("stream/" + topic);
            return SubscribeAsync(apiClient, url, request, null, false, onData, ct);
        }

        protected override bool HandleSubscriptionResponse(SocketConnection socketConnection, SocketSubscription subscription, object request, JToken message, out CallResult<object>? callResult)
        {
            callResult = null;
            if (message.Type != JTokenType.Object)
                return false;

            //var result = message["result"];
            //if (result != null && result.Type == JTokenType.Null)
            //{
            //    log.Write(LogLevel.Trace, $"Socket {socketConnection.Socket.Id} Subscription completed");
            //    callResult = new CallResult<object>(new object());
            //    return true;
            //}
            log.Write(LogLevel.Trace, $"Socket {socketConnection.Socket.Id} Subscription completed");
            var p = ProcessTokenData(message);
            var firstData = Deserialize<LunoEventOrderBook>(p);
            OnSubscriptionDataReceived?.Invoke(this, firstData);
            callResult = new CallResult<object>(firstData);
            return true;

            var error = message["error"];
            if (error == null)
            {
                callResult = new CallResult<object>(new ServerError("Unknown error: " + message));
                return true;
            }

            callResult = new CallResult<object>(new ServerError(error["code"]!.Value<int>(), error["msg"]!.ToString()));
            return true;
        }

        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, object request)
        {
            return true;
            //if (message.Type != JTokenType.Object)
            //    return false;

            //var bRequest = (LunoSocketRequest)request;
            //var stream = message["stream"];
            //if (stream == null)
            //    return false;

            //return bRequest.Params.Contains(stream.ToString());
        }

        protected override bool MessageMatchesHandler(SocketConnection socketConnection, JToken message, string identifier)
        {
            return true;
        }

        protected override Task<CallResult<bool>> AuthenticateSocketAsync(SocketConnection socketConnection)
        {
            throw new NotImplementedException();
        }

        protected override async Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription subscriptionToUnsub)
        {
            return true;
            //var topics = ((LunoSocketRequest)subscriptionToUnsub.Request!).Params;
            //var unsub = new LunoSocketRequest { Method = "UNSUBSCRIBE", Params = topics, Id = NextId() };
            //var result = false;

            //if (!connection.Socket.IsOpen)
            //    return true;

            //await connection.SendAndWaitAsync(unsub, ClientOptions.SocketResponseTimeout, data =>
            //{
            //    if (data.Type != JTokenType.Object)
            //        return false;

            //    var id = data["id"];
            //    if (id == null)
            //        return false;

            //    if ((int)id != unsub.Id)
            //        return false;

            //    var result = data["result"];
            //    if (result?.Type == JTokenType.Null)
            //    {
            //        result = true;
            //        return true;
            //    }

            //    return true;
            //}).ConfigureAwait(false);
            //return result;
        }

        protected override bool HandleQueryResponse<T>(SocketConnection socketConnection, object request, JToken data, [NotNullWhen(true)] out CallResult<T>? callResult)
        {
            throw new NotImplementedException();
        }
    }
}
