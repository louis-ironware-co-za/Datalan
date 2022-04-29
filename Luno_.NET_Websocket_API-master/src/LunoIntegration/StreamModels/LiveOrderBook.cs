namespace LunoIntegration.StreamModels
{
    using LunoIntegration.Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Collections.Generic;

    public class LiveOrderBook
    {
        [JsonProperty("sequence")]
        public double Sequence { get; set; }

        [JsonProperty("bids")]
        public List<LiveOrder> Bids { get; set; }

        [JsonProperty("asks")]
        public List<LiveOrder> Asks { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TradingStatus Status { get; set; }
    }
}
