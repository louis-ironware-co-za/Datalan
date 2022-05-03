using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.Models
{
    public class MarketListEntry
    {
        [JsonProperty("marketId")]
        public int MarketId { get; set; }

        [JsonProperty("marketName")]
        public string MarketName { get; set; }

        [JsonProperty("exchId")]
        public int ExchId { get; set; }

        [JsonProperty("exchCode")]
        public string ExchCode { get; set; }

        [JsonProperty("exchName")]
        public string ExchName { get; set; }

        [JsonProperty("baseCurrCode")]
        public string BaseCurrCode { get; set; }

        [JsonProperty("baseCurrId")]
        public int BaseCurrId { get; set; }

        [JsonProperty("quoteCurrCode")]
        public string QuoteCurrCode { get; set; }

        [JsonProperty("quoteCurrId")]
        public int QuoteCurrId { get; set; }

        [JsonProperty("baseCurrName")]
        public string BaseCurrName { get; set; }

        [JsonProperty("quoteCurrName")]
        public string QuoteCurrName { get; set; }

        [JsonProperty("exchmktId")]
        public int ExchmktId { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("baseIsFiat")]
        public bool BaseIsFiat { get; set; }

        [JsonProperty("quoteIsFiat")]
        public bool QuoteIsFiat { get; set; }

        [JsonProperty("basePricePrecision")]
        public int BasePricePrecision { get; set; }

        [JsonProperty("baseQuantityPrecision")]
        public int BaseQuantityPrecision { get; set; }

        [JsonProperty("quotePricePrecision")]
        public int QuotePricePrecision { get; set; }

        [JsonProperty("quoteQuantityPrecision")]
        public int QuoteQuantityPrecision { get; set; }

        [JsonProperty("primaryCurrCode")]
        public string PrimaryCurrCode { get; set; }

        [JsonProperty("secondaryCurrCode")]
        public string SecondaryCurrCode { get; set; }

        [JsonProperty("baseHasImage")]
        public bool BaseHasImage { get; set; }

        [JsonProperty("quoteHasImage")]
        public bool QuoteHasImage { get; set; }

        [JsonProperty("baseImageName")]
        public string BaseImageName { get; set; }

        [JsonProperty("quoteImageName")]
        public string QuoteImageName { get; set; }
    }
}
