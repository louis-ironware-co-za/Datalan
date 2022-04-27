using CryptoExchange.Net.Converters;
using CryptoExchange.Net.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects.Models
{
    //[JsonConverter(typeof(ArrayConverter))]
    public class LunoOrderBookEntry : ISymbolOrderBookEntry
    {
        [ArrayProperty(0)]
        public string Id { get; set; }
        /// <summary>
        /// The volume available at the limit price
        /// </summary>
        [ArrayProperty(2)]
        public decimal Quantity {get;set;}
        /// <summary>
        /// Limit price at which orders are trading at
        /// </summary>
        [ArrayProperty(1)]
        public decimal Price {get;set;}
    }
}
