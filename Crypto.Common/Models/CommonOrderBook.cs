using Crypto.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Models
{
    public class CommonOrderBook
    {
        public List<CommonOrderItem> Buys { get; set; } = new List<CommonOrderItem>();
        public List<CommonOrderItem> Sells { get; set;} = new List<CommonOrderItem>();

        public ICryptoExchange CryptoExchange { get; private set; }

        public CommonOrderBook(ICryptoExchange cryptoExchange)
        {
            this.CryptoExchange = cryptoExchange;
        }
    }
}
