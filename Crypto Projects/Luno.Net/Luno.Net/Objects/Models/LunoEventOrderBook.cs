using CryptoExchange.Net.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects.Models
{
    public class LunoEventOrderBook
    {
        public long Sequence { get; set; }
        public IEnumerable<LunoEventOrderBookTrade> Trade_Updates { get; set; } = Array.Empty<LunoEventOrderBookTrade>();
        public LunoEventOrderBookCreate? Create_Update { get; set; }
        public LunoEventOrderBookDelete? Delete_Update { get; set; }
        public LunoEventOrderBookStatus? Status_Update { get; set; }
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? TimeStamp { get; set; }
        public IEnumerable<LunoOrderBookEntry> Asks { get; set; } = null;
        public IEnumerable<LunoOrderBookEntry> Bids { get; set; } = null;
    }
}
