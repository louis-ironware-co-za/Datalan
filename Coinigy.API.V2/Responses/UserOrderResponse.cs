using Coinigy.API.V2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.Responses
{
    public class UserOrderResponse
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }

        [JsonProperty("authId")]
        public int AuthId { get; set; }

        [JsonProperty("baseCurrency")]
        public string BaseCurrency { get; set; }

        [JsonProperty("quoteCurrency")]
        public string QuoteCurrency { get; set; }

        [JsonProperty("market")]
        public string Market { get; set; }

        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        [JsonProperty("quantityRemaining")]
        public decimal QuantityRemaining { get; set; }

        [JsonProperty("limitPrice")]
        public decimal LimitPrice { get; set; }

        [JsonProperty("stopPrice")]
        public decimal? StopPrice { get; set; }

        [JsonProperty("conditionalOperator")]
        public int ConditionalOperator { get; set; }

        [JsonProperty("openTime")]
        public DateTime? OpenTime { get; set; }

        [JsonProperty("closeTime")]
        public DateTime? CloseTime { get; set; }

        [JsonProperty("updateTime")]
        public DateTime? UpdateTime { get; set; }

        [JsonProperty("foreignOrderId")]
        public string ForeignOrderId { get; set; }

        [JsonProperty("executedPrice")]
        public decimal? ExecutedPrice { get; set; }

        [JsonProperty("untrackedOrder")]
        public int? UntrackedOrder { get; set; }

        [JsonProperty("side")]
        public int Side { get; set; }

        [JsonProperty("orderType")]
        public int OrderType { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("exchmktId")]
        public int? ExchmktId { get; set; }

        [JsonProperty("exchange")]
        public string Exchange { get; set; }

        [JsonProperty("exchCode")]
        public string ExchCode { get; set; }

        [JsonProperty("coinigyOrderId")]
        public int? CoinigyOrderId { get; set; }

        [JsonProperty("stopOrder")]
        public StopOrder StopOrder { get; set; }

        [JsonProperty("authNickname")]
        public string AuthNickname { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("fee")]
        public decimal? Fee { get; set; }

        [JsonProperty("authVersion")]
        public int? AuthVersion { get; set; }

        [JsonProperty("conditionalOperatorString")]
        public string ConditionalOperatorString { get; set; }

        [JsonProperty("trades")]
        public List<Trade> Trades { get; set; }
    }
}
