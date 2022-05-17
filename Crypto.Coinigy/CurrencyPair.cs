using Coinigy.API.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Coinigy
{
    public  class CurrencyPair
    {
        public string Currency1 { get; set; }
        public string Currency2 { get; set; }

        public static implicit operator CurrencyPair(MarketListEntry market)
        {
            if (!string.IsNullOrEmpty(market.MarketName))
            {
                var split = market.MarketName.Split("/".ToCharArray());

                return new CurrencyPair()
                {
                    Currency1 = split[0],
                    Currency2 = split[1]
                };
            }
            else
            {
                return new CurrencyPair()
                {
                    Currency1 = market.BaseCurrCode,
                    Currency2 = market.QuoteCurrCode,
                };
            }
            
        }
    }
}
