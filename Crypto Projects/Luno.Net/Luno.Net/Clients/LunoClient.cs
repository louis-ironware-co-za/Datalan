using CryptoExchange.Net;
using CryptoExchange.Net.Objects;
using Luno.Net.Clients.GeneralApi;
using Luno.Net.Interfaces.Clients;
using Luno.Net.Interfaces.Clients.GeneralApi;
using Luno.Net.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Luno.Net.Clients
{
    public class LunoClient : BaseRestClient, ILunoClient
    {
        public ILunoClientGeneralApi GeneralApi { get; }

        public LunoClient() : this(LunoClientOptions.Default)
        {

        }

        public LunoClient(LunoClientOptions options):base("Luno", options)
        {
            GeneralApi = AddApiClient(new LunoClientGeneralApi(log, this, options));

            requestBodyEmptyContent = "";
            requestBodyFormat = CryptoExchange.Net.Objects.RequestBodyFormat.FormData;
            arraySerialization = CryptoExchange.Net.Objects.ArrayParametersSerialization.MultipleValues;
        }

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options">Options to use as default</param>
        public static void SetDefaultOptions(LunoClientOptions options)
        {
            LunoClientOptions.Default = options;
        }

        /// <inheritdoc />
        protected override Error ParseErrorResponse(JToken error)
        {
            if (!error.HasValues)
                return new ServerError(error.ToString());

            if (error["msg"] == null && error["code"] == null)
                return new ServerError(error.ToString());

            if (error["msg"] != null && error["code"] == null)
                return new ServerError((string)error["msg"]!);

            return new ServerError((int)error["code"]!, (string)error["msg"]!);
        }

        internal Task<WebCallResult<T>> SendRequestInternal<T>(RestApiClient apiClient, Uri uri, HttpMethod method, CancellationToken cancellationToken,
            Dictionary<string, object>? parameters = null, bool signed = false, HttpMethodParameterPosition? postPosition = null,
            ArrayParametersSerialization? arraySerialization = null) where T : class
        {
            return base.SendRequestAsync<T>(apiClient, uri, method, cancellationToken, parameters, signed, postPosition, arraySerialization);
        }
    }
}
