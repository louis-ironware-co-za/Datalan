using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using Luno.Net.Interfaces.Clients.GeneralApi;
using Luno.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luno.Net.Clients.GeneralApi
{
    public class LunoClientGeneralApiGeneral : ILunoClientGeneralApiGeneral
    {
        private const string lunoAPI = "api";
        private const string apiVersion = "1";
        private const string lunoExchangeAPI = "api/exchange";
        private const string lunoExchangeVersion = "1";

        private const string listAccountsEndPoint = "balance";
        private const string marketsInfoEndPoint = "markets";

        private readonly LunoClientGeneralApi _baseClient;

        internal LunoClientGeneralApiGeneral(LunoClientGeneralApi baseClient)
        {
            _baseClient = baseClient;
        }

        public async Task<WebCallResult<LunoAccountInfo>> GetAccountInfoAsync(long? receiveWindow, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<LunoAccountInfo>(_baseClient.GetUrl(listAccountsEndPoint, lunoAPI, apiVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }

        public async Task<WebCallResult<LunoMarket>> GetExchangeSymbolsAsync(long? receiveWindow, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.Options.ReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await _baseClient.SendRequestInternal<LunoMarket>(_baseClient.GetUrl(marketsInfoEndPoint, lunoExchangeAPI, lunoExchangeVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
        }
    }
}
