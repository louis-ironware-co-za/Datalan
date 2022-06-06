using Crypto.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Models
{
    public class CommonAccountBalance
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

        /// <summary>
        /// In coinigy this would be the authid and Luno would be accountid
        /// </summary>
        public string AuthId { get; set; }
        public string AuthKey { get; set; }
        public string AuthNickname { get; set; }
        /// <summary>
        /// Asset in Luno.
        /// </summary>
        public string BalanceCurrCode { get; set; }
        public string BalanceQuoteCurrCode { get; set; }
        public double BalanceAmountAvailable { get;set; }
        public double BalanceAmountHeld { get; set; }

        public object Tag { get; set; }
    }
}
