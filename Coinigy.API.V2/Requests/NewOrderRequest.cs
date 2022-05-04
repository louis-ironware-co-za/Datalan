using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.Requests
{
    public class NewOrderRequest
    {
        [JsonProperty("exchMktId")]
        public int ExchMktId { get; set; }

        [JsonProperty("side")]
        public int Side { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        [JsonProperty("priceType")]
        public int PriceType { get; set; }

        [JsonProperty("conditionalOperator")]
        public int ConditionalOperator { get; set; }

        [JsonProperty("triggerPrice")]
        public decimal? TriggerPrice { get; set; }

        [JsonProperty("triggerExchMktId")]
        public int? TriggerExchMktId { get; set; }

        [JsonProperty("expiresAt")]
        public DateTime? ExpiresAt { get; set; }

        [JsonProperty("trailingType")]
        public int TrailingType { get; set; }

        [JsonProperty("trailingGap")]
        public decimal? TrailingGap { get; set; }
    }
}
