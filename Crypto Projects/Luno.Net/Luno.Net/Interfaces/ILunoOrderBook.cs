using Luno.Net.Converters;
using Luno.Net.Enums;
using Luno.Net.Objects.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Interfaces
{
    public interface ILunoOrderBook
    {
        long Sequence { get; set; }
        /// <summary>
        /// Status of the market. (only filled from stream updates)
        /// </summary>
        [JsonConverter(typeof(SymbolStatusConverter))]
        SymbolStatus Status { get; set; }
        /// <summary>
        /// The symbol of the order book (only filled from stream updates)
        /// </summary>
        string Asset { get; set; }
        DateTime TimeStamp { get; set; }

        IEnumerable<LunoOrderBookEntry> Asks { get;set; }
        IEnumerable<LunoOrderBookEntry> Bids { get; set; }
    }

}
