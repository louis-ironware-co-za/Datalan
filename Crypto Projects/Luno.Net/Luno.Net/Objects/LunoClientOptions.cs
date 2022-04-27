using CryptoExchange.Net.Objects;
using Luno.Net.Interfaces.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects
{
    public class LunoClientOptions : BaseRestClientOptions
    {
        public static LunoClientOptions Default { get; set; } = new LunoClientOptions();

        /// <summary>
        /// The default receive window for requests
        /// </summary>
        public TimeSpan ReceiveWindow { get; set; } = TimeSpan.FromSeconds(5);

        private LunoApiClientOptions _generalApiOptions = new LunoApiClientOptions(LunoApiAddresses.Default.RestClientAddress)
        {
            AutoTimestamp = true,
            RateLimiters = new List<CryptoExchange.Net.Interfaces.IRateLimiter>()
            {
                new RateLimiter()
                .AddPartialEndpointLimit("/api/", 300, TimeSpan.FromMinutes(1)),
            }
        };

        public LunoApiClientOptions GeneralApiOptions
        {
            get => _generalApiOptions;
            set => _generalApiOptions = new LunoApiClientOptions(_generalApiOptions, value);
        }

        public LunoClientOptions() : this(Default)
        {

        }


        internal LunoClientOptions(LunoClientOptions baseOn) : base(baseOn)
        {
            if (baseOn == null)
            {
                return;
            }

            _generalApiOptions = new LunoApiClientOptions(baseOn.GeneralApiOptions, null);
        }
    }

    public class LunoSocketClientOptions : BaseSocketClientOptions
    {
        public static LunoSocketClientOptions Default { get; set; } = new LunoSocketClientOptions()
        {
            SocketSubscriptionsCombineTarget = 10,
        };

        private ApiClientOptions _generalStreamsOptions = new ApiClientOptions(LunoApiAddresses.Default.SocketClientAddress);
        public ApiClientOptions GeneralStreamsOptions
        {
            get => _generalStreamsOptions;
            set => _generalStreamsOptions = new ApiClientOptions(_generalStreamsOptions, value);
        }

        public LunoSocketClientOptions() : this(Default)
        {

        }

        public LunoSocketClientOptions(LunoSocketClientOptions baseOn) : base(baseOn)
        {
            if (baseOn == null)
            {
                return;
            }

            _generalStreamsOptions = new ApiClientOptions(baseOn.GeneralStreamsOptions, null);
        }
    }

    public class LunoApiClientOptions : RestApiClientOptions
    {
        /// <summary>
        /// A manual offset for the timestamp. Should only be used if AutoTimestamp and regular time synchronization on the OS is not reliable enough
        /// </summary>
        public TimeSpan TimestampOffset { get; set; } = TimeSpan.Zero;

        public LunoApiClientOptions()
        {

        }

        internal LunoApiClientOptions(string baseAddress) : base(baseAddress)
        {

        }

        internal LunoApiClientOptions(LunoApiClientOptions baseOn, LunoApiClientOptions? newValues):base(baseOn, newValues)
        {
            TimestampOffset = newValues?.TimestampOffset ?? baseOn.TimestampOffset;
        }
    }

    public class LunoOrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// The top amount of results to keep in sync. If for example limit=10 is used, the order book will contain the 10 best bids and 10 best asks. Leaving this null will sync the full order book
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Update interval in milliseconds, either 100 or 1000. Defaults to 1000
        /// </summary>
        public int? UpdateInterval { get; set; }

        /// <summary>
        /// The rest client to use for requesting the initial order book
        /// </summary>
        public ILunoClient? RestClient { get; set; }

        /// <summary>
        /// The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.
        /// </summary>
        public ILunoSocketClient? SocketClient { get; set; }
    }
}
