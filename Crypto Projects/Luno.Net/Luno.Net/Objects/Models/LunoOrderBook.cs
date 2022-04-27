using Luno.Net.Converters;
using Luno.Net.Enums;
using Luno.Net.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects.Models
{
    public class LunoOrderBook : ILunoOrderBook
    {
        public long Sequence {get;set;}
        [JsonConverter(typeof(SymbolStatusConverter))]
        public SymbolStatus Status {get;set; }
        public string Asset { get; set; } = String.Empty;
        public DateTime TimeStamp { get; set; }
        public IEnumerable<LunoOrderBookEntry> Asks { get; set; } = Array.Empty<LunoOrderBookEntry>();
        public IEnumerable<LunoOrderBookEntry> Bids { get; set; } = Array.Empty<LunoOrderBookEntry>();
    }
}
