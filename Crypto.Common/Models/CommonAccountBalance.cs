using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Models
{
    public class CommonAccountBalance
    {
        /// <summary>
        /// In coinigy this would be the authid and Luno would be accountid
        /// </summary>
        public string AuthId { get; set; }
        public string BalanceCurrCode { get; set; }
        public string BalanceQuoteCurrCode { get; set; }
        public double BalanceAmountAvailable { get;set; }
        public double BalanceAmountHeld { get; set; }
    }
}
