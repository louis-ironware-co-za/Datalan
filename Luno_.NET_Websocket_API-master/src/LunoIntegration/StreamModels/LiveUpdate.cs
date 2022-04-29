namespace LunoIntegration.StreamModels
{
    using LunoIntegration.Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class LiveUpdate
    {
        [JsonProperty("sequence")]
        public double Sequence { get; set; }

        [JsonProperty("trade_updates")]
        public TradeUpdate[] TradeUpdates { get; set; }

        [JsonProperty("create_update")]
        public CreateUpdate CreateUpdate { get; set; }

        [JsonProperty("delete_update")]
        public DeleteUpdate DeleteUpdate { get; set; }

        [JsonProperty("timestamp")]
        public double TimeStamp { get; set; }

        [JsonProperty("status_update")]
        public string StatusUpdate { get; set; }
    }
}
