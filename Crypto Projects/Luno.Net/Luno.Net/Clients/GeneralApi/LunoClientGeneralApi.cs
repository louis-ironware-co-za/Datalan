using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Interfaces.CommonClients;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Luno.Net.Interfaces.Clients.GeneralApi;
using Luno.Net.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luno.Net.Clients.GeneralApi
{
    public class LunoClientGeneralApi :RestApiClient, ILunoClientGeneralApi, ISpotClient
    {
        private readonly LunoClient _baseClient;
        internal new readonly LunoClientOptions Options;

        internal static TimeSyncState TimeSyncState = new TimeSyncState();

        private readonly Log _log;

        public event Action<OrderId> OnOrderPlaced;
        public event Action<OrderId> OnOrderCanceled;

        public ILunoClientGeneralApiGeneral General { get; }
        public string ExchangeName => "Luno";

        public LunoClientGeneralApi(Log log, LunoClient baseClient, LunoClientOptions options):base(options, options.GeneralApiOptions)
        {
            Options = options;
            _log = log;
            _baseClient = baseClient;

            General = new LunoClientGeneralApiGeneral(this);
        }

        protected override TimeSyncInfo GetTimeSyncInfo()
        {
            return new TimeSyncInfo(_log, Options.GeneralApiOptions.AutoTimestamp, Options.GeneralApiOptions.TimestampRecalculationInterval, TimeSyncState);
        }

        public override TimeSpan GetTimeOffset()
        {
            return TimeSyncState.TimeOffset;
        }

        protected override Task<WebCallResult<DateTime>> GetServerTimestampAsync()
        {
            return GetFakeServerTimestamp();
        }

        internal async Task<WebCallResult<DateTime>> GetFakeServerTimestamp()
        {
            return new WebCallResult<DateTime>(null, null, null, null, null, null, null, null, DateTime.Now, null);

        }

        protected override AuthenticationProvider CreateAuthenticationProvider(ApiCredentials credentials)
        {
            return new LunoAuthenticationProvider(credentials);
        }

        public Task<WebCallResult<OrderId>> PlaceOrderAsync(string symbol, CommonOrderSide side, CommonOrderType type, decimal quantity, decimal? price = null, string? accountId = null, string? clientOrderId = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public string GetSymbolName(string baseAsset, string quoteAsset)
        {
            return (baseAsset + quoteAsset).ToUpper(CultureInfo.InvariantCulture);
        }

        public Task<WebCallResult<IEnumerable<Symbol>>> GetSymbolsAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<WebCallResult<Ticker>> GetTickerAsync(string symbol, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<WebCallResult<IEnumerable<Ticker>>> GetTickersAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<WebCallResult<IEnumerable<Kline>>> GetKlinesAsync(string symbol, TimeSpan timespan, DateTime? startTime = null, DateTime? endTime = null, int? limit = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<WebCallResult<OrderBook>> GetOrderBookAsync(string symbol, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<WebCallResult<IEnumerable<Trade>>> GetRecentTradesAsync(string symbol, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<WebCallResult<IEnumerable<Balance>>> GetBalancesAsync(string? accountId = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<WebCallResult<Order>> GetOrderAsync(string orderId, string? symbol = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<WebCallResult<IEnumerable<UserTrade>>> GetOrderTradesAsync(string orderId, string? symbol = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<WebCallResult<IEnumerable<Order>>> GetOpenOrdersAsync(string? symbol = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<WebCallResult<IEnumerable<Order>>> GetClosedOrdersAsync(string? symbol = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<WebCallResult<OrderId>> CancelOrderAsync(string orderId, string? symbol = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        internal async Task<WebCallResult<T>> SendRequestInternal<T>(Uri uri, HttpMethod method, CancellationToken cancellationToken,
            Dictionary<string, object>? parameters = null, bool signed = false, HttpMethodParameterPosition? postPosition = null,
            ArrayParametersSerialization? arraySerialization = null) where T : class
        {
            return await _baseClient.SendRequestInternal<T>(this, uri, method, cancellationToken, parameters, signed, postPosition, arraySerialization).ConfigureAwait(false);
        }

        internal Uri GetUrl(string endpoint, string api, string? version = null)
        {
            var result = BaseAddress.AppendPath(api);

            if (!string.IsNullOrEmpty(version))
                result = result.AppendPath($"{version}");

            return new Uri(result.AppendPath(endpoint));
        }
    }
}
