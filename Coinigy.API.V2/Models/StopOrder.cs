using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.Models
{
    public class StopOrder
    {
        [JsonProperty("stopOrderId")]
        public int StopOrderId { get; set; }

        [JsonProperty("exchMktId")]
        public int ExchMktId { get; set; }

        [JsonProperty("stopPrice")]
        public int StopPrice { get; set; }

        [JsonProperty("openTime")]
        public DateTime OpenTime { get; set; }

        [JsonProperty("coinigyOrderId")]
        public int CoinigyOrderId { get; set; }

        [JsonProperty("condition")]
        public int Condition { get; set; }

        [JsonProperty("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        [JsonProperty("trailingGap")]
        public int? TrailingGap { get; set; }

        [JsonProperty("trailingType")]
        public int TrailingType { get; set; }
    }
}
