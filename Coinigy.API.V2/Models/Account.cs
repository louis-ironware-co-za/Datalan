using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.Models
{
    public class Account
    {
        [JsonProperty("authKey")]
        public string AuthKey { get; set; }

        [JsonProperty("authOptional")]
        public string AuthOptional { get; set; }

        [JsonProperty("authExchId")]
        public int AuthExchId { get; set; }

        [JsonProperty("authNickname")]
        public string AuthNickname { get; set; }

        [JsonProperty("authActive")]
        public bool AuthActive { get; set; }

        [JsonProperty("authTrade")]
        public bool AuthTrade { get; set; }

        [JsonProperty("authId")]
        public int AuthId { get; set; }

        [JsonProperty("authUserId")]
        public int AuthUserId { get; set; }

        [JsonProperty("authAdded")]
        public DateTime AuthAdded { get; set; }

        [JsonProperty("authUpdated")]
        public DateTime AuthUpdated { get; set; }

        [JsonProperty("authFetched")]
        public DateTime AuthFetched { get; set; }

        [JsonProperty("authTradingType")]
        public string AuthTradingType { get; set; }

        [JsonProperty("authHasSecondary")]
        public bool AuthHasSecondary { get; set; }

        [JsonProperty("authVersion")]
        public int AuthVersion { get; set; }

        [JsonProperty("ordersRefreshed")]
        public DateTime OrdersRefreshed { get; set; }

        [JsonProperty("balancesRefreshed")]
        public DateTime BalancesRefreshed { get; set; }
    }
}
