using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Sockets;
using Luno.Net.Interfaces;
using Luno.Net.Interfaces.Clients.GeneralApi;
using Luno.Net.Objects;
using Luno.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luno.Net.Clients.GeneralApi
{
    public class LunoSocketClientGeneralStreams : SocketApiClient, ILunoSocketClientGeneralStreams
    {
        private readonly LunoSocketClient _baseClient;
        private readonly Log _log;

        private const string marketStreamEndpoint = "stream";

        public LunoSocketClientGeneralStreams(Log log, LunoSocketClient baseClient, LunoSocketClientOptions options):base(options, options.GeneralStreamsOptions)
        {
            _baseClient = baseClient;
            _log = log;
        }

        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
        {
            return new LunoAuthenticationProvider(credentials);
        }

        public async Task<CallResult<UpdateSubscription>> SubscribeToMarketStreamAsync(string symbol, string apiKey, string apiSecret, Action<DataEvent<LunoEventOrderBook>> onMessage, CancellationToken ct = default)
        {
            symbol.ValidateNotNull(nameof(symbol));

            //TODO: Implement validate symbol for Luno
            // foreach (var symbol in symbols)
            //symbol.ValidateBinanceSymbol();

            var handler = new Action<DataEvent<LunoEventOrderBook>>(data =>
            {
                onMessage(data.As(data.Data));
            });

            return await _baseClient.SubscribeInternal(this, BaseAddress, symbol, apiKey, apiSecret, handler, ct).ConfigureAwait(false);
        }

    }
}
