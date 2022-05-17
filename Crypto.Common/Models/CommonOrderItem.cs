using Crypto.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Models
{
    public class CommonOrderItem
    {
        [Browsable(false)]
        public CommonMarket MarketRef { get; set; }
        public string MarketRefName { get => MarketRef?.MarketName; }
        [Browsable(false)]
        public ICryptoExchange ExchangeRef { get; set; }
        public string ExchangeName
        {
            get
            {
                return ExchangeRef?.ExchangeName;
            }
        }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string OrderType { get; set; }
    }
}
