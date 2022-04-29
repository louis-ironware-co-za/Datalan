using LunoIntegration.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LunoIntegration.Models
{
    public class MarketInfo
    {
        [JsonProperty(PropertyName = "base_currency")]
        public string BaseCurrency { get; set; }

        [JsonProperty(PropertyName = "counter_currency")]
        public string CounterCurrency { get; set; }

        [JsonProperty(PropertyName = "fee_scale")]
        public int FeeScale { get; set; }

        [JsonProperty(PropertyName = "market_id")]
        public string MarketId { get; set; }

        [JsonProperty(PropertyName = "max_price")]
        public decimal MaxPrice { get; set; }
        
        [JsonProperty(PropertyName = "max_volume")]
        public decimal MaxVolume { get; set; }

        [JsonProperty(PropertyName = "min_price")]
        public decimal MinPrice { get; set; }

        [JsonProperty(PropertyName = "min_volume")]
        public decimal MinVolume { get; set; }
        
        [JsonProperty(PropertyName = "price_scale")]
        public int PriceScale { get; set; }

        [JsonProperty(PropertyName = "trading_status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TradingStatus TradingStatus { get; set; }

        [JsonProperty(PropertyName = "volume_scale")]
        public int VolumeScale { get; set; }
    }
}
