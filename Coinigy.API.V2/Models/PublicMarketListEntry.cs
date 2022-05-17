using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.Models
{
    public class PublicMarketListEntry
    {
        [JsonProperty("exchmktId")]
        public int ExchmktId { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("exchId")]
        public int? ExchId { get; set; }

        [JsonProperty("exchName")]
        public string ExchName { get; set; }

        [JsonProperty("exchCode")]
        public string ExchCode { get; set; }

        [JsonProperty("exchSlug")]
        public string ExchSlug { get; set; }

        [JsonProperty("marketId")]
        public int? MarketId { get; set; }

        [JsonProperty("marketName")]
        public string MarketName { get; set; }

        [JsonProperty("baseCurrencyCode")]
        public string BaseCurrencyCode { get; set; }

        [JsonProperty("baseCurrencyId")]
        public int? BaseCurrencyId { get; set; }

        [JsonProperty("quoteCurrencyCode")]
        public string QuoteCurrencyCode { get; set; }

        [JsonProperty("quoteCurrencyId")]
        public int? QuoteCurrencyId { get; set; }

        [JsonProperty("baseIsFiat")]
        public bool BaseIsFiat { get; set; }

        [JsonProperty("quoteIsFiat")]
        public bool QuoteIsFiat { get; set; }

        [JsonProperty("baseCurrencyName")]
        public string BaseCurrencyName { get; set; }

        [JsonProperty("quoteCurrencyName")]
        public string QuoteCurrencyName { get; set; }

        [JsonProperty("quantityPrecision")]
        public int? QuantityPrecision { get; set; }

        [JsonProperty("pricePrecision")]
        public int? PricePrecision { get; set; }
    }
}
