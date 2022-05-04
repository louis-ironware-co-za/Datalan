using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.Models
{
    public class Trade
    {
        [JsonProperty("coinigyTradeId")]
        public int? CoinigyTradeId { get; set; }

        [JsonProperty("coinigyOrderId")]
        public int? CoinigyOrderId { get; set; }

        [JsonProperty("foreignOrderId")]
        public string ForeignOrderId { get; set; }

        [JsonProperty("foreignTradeId")]
        public string ForeignTradeId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("closeTime")]
        public DateTime? CloseTime { get; set; }
    }
}
