using Crypto.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Models
{
    public class CommonTradeItem
    {
        [Browsable(false)]
        public CommonMarket MarketRef { get; set; }
        public string MarketRefName { get => MarketRef?.MarketName; }
        [Browsable(false)]
        public ICryptoExchange ExchangeRef { get; set; }
        public string ExchangeRefName { get => ExchangeRef?.ExchangeName; }
        public string Exchange { get; set; }
        public int MarketId { get;set; }
        public int ExchId { get;set;}
        public string Type { get; set;}
        public string OrderId { get; set; }
        public string TradeId { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
    }
}
