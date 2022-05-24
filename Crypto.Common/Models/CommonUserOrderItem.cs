using Crypto.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Models
{
    public class CommonUserOrderItem
    {
        [Browsable(false)]
        public ICryptoExchange ExchangeRef { get; set; }
        public string ExchangeName
        {
            get
            {
                return ExchangeRef?.ExchangeName;
            }
        }

        public int AuthId { get; set; }
        public string Market { get; set; }
        public int Side { get; set; }
        public string Type { get; set; }
        public decimal? Fee { get; set; }
        public decimal FeeBase { get; set; }
        /// <summary>
        /// AKA Counter in Luno.
        /// </summary>
        public decimal FeeQuote { get; set; }
        public string ExchCode { get; set; }
        public int? ExchmktId { get; set; }
        /// <summary>
        /// In coinigy refers to the coinigy order id.
        /// </summary>
        public string OrderId { get; set; }
        public decimal LimitPrice { get; set; }

        public decimal Quantity { get; set; }

        //Luno
        public decimal Base { get; set; }
        public decimal Counter { get; set; }
    }
}
