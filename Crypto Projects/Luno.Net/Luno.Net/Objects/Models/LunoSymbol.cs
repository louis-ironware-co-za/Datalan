using Luno.Net.Converters;
using Luno.Net.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects.Models
{
    public  class LunoSymbol
    {
        [JsonProperty("market_id")]
        public string Name { get; set; } = string.Empty;
        public string BaseCurrency { get; set; } = string.Empty;
        public string CounterCurrency { get; set; } = string.Empty;
        /// <summary>
        /// Fee decimal places
        /// </summary>
        public int FeeScale { get; set; }
        /// <summary>
        /// Maximum order price
        /// </summary>
        public decimal MaxPrice { get; set; }
        /// <summary>
        /// Maximum order volume
        /// </summary>
        public decimal MaxVolume { get; set; }
        /// <summary>
        /// Minimum order price
        /// </summary>
        public decimal MinPrice { get; set; }
        /// <summary>
        /// Minimum order volume
        /// </summary>
        public decimal MinVolume { get; set; }
        /// <summary>
        /// Current market trading status
        /// </summary>
        /// 
        [JsonProperty("trading_status")]
        [JsonConverter(typeof(SymbolStatusConverter))]
        public SymbolStatus TradingStatus { get; set; }
        /// <summary>
        /// Price decimal places
        /// </summary>
        public decimal PriceScale { get; set; }
        /// <summary>
        /// Volume decimal places
        /// </summary>
        public decimal VolumeScale { get; set; }

    }
}
