namespace DxAimArb.Data
{
    public class TradeRow
    {
        public long market_history_id { get; set; }  
        public string? exchange { get; set; }
        public int marketid { get; set; }
        public string? label { get; set; }
        public string? tradeid { get; set; }
        public string? time { get; set; }
        public long? price { get; set; }
        public long? quantity { get; set; }
        public long? total { get; set; }
        public string? timestamp { get; set; }
        public string? time_local { get; set; }
        public string? type { get; set; }
        public int exchId { get; set; }
        public string? channel { get; set; }

    }
}
