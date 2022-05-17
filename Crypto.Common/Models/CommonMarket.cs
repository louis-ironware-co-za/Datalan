using Crypto.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Models
{
    public class CommonMarket
    {
        [Browsable(false)]
        public ICryptoExchange ExchangeRef { get; set; }

        public Exchanges ExchangeName => ExchangeRef.Exchange;

        /// <summary>
        /// Market id is int in Coinigy and string in Luno.
        /// </summary>
        public string MarketIdString { get; set; }
        /// <summary>
        /// Market id is int in Coinigy and string in Luno.
        /// </summary>
        public int MarketIdInt { get; set; }
        public string BaseCurrency { get; set; }
        /// <summary>
        /// Also known as CounterCurrency
        /// </summary>
        public string QuoteCurrency { get; set; }
        public int? ExchmktId { get; set; }
        public string ExchCode { get; set; }
        public string Status { get; set; }
        public string MarketName { get; set; }
    }
}
