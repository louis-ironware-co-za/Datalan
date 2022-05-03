using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.Models
{
    public class PublicExchange
    {
        [JsonProperty("exchJson")]
        public string ExchJson { get; set; }

        [JsonProperty("marketCount")]
        public int MarketCount { get; set; }

        [JsonProperty("currencyCount")]
        public int CurrencyCount { get; set; }

        [JsonProperty("quoteCount")]
        public int QuoteCount { get; set; }

        [JsonProperty("heatIndex")]
        public int HeatIndex { get; set; }

        [JsonProperty("exchId")]
        public int ExchId { get; set; }

        [JsonProperty("exchName")]
        public string ExchName { get; set; }

        [JsonProperty("exchCode")]
        public string ExchCode { get; set; }

        [JsonProperty("exchUrl")]
        public string ExchUrl { get; set; }

        [JsonProperty("exchBalanceEnabledV2")]
        public bool ExchBalanceEnabledV2 { get; set; }

        [JsonProperty("exchTradeEnabledV2")]
        public bool ExchTradeEnabledV2 { get; set; }

        [JsonProperty("hasSpotTrading")]
        public bool HasSpotTrading { get; set; }

        [JsonProperty("hasFuturesTrading")]
        public bool HasFuturesTrading { get; set; }

        [JsonProperty("exchVolume24")]
        public int ExchVolume24 { get; set; }

        [JsonProperty("exchVolume1")]
        public int ExchVolume1 { get; set; }

        [JsonProperty("favoritesScore")]
        public int FavoritesScore { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("exchMarketsInverted")]
        public bool ExchMarketsInverted { get; set; }

        [JsonProperty("trafficRank")]
        public int TrafficRank { get; set; }

        [JsonProperty("trafficRank90dChange")]
        public int TrafficRank90dChange { get; set; }

        [JsonProperty("exchHasWhitelisting")]
        public bool ExchHasWhitelisting { get; set; }
    }
}
