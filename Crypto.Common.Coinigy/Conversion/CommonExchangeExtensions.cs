using Coinigy.API.V2.Models;
using Crypto.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Coinigy.Conversion
{
    public static class CommonExchangeExtensions
    {
        public static Exchange ToExchange(this CommonExchange exchange)
        {
            if (exchange == null)
            {
                return null;
            }

            Exchange toreturn = new Exchange()
            {
                ExchFee = exchange.ExchFee,
                ExchFeeBuy = exchange.ExchFeeBuy,
                ExchCode = exchange.ExchCode,
                ExchFeeSell = exchange.ExchFeeSell,
                ExchName = exchange.ExchName,
                ExchTradeEnabled = exchange.ExchTradeEnabled,
                ExchEnabled = exchange.ExchEnabled,
            };

            return toreturn;
        }

        public static CommonExchange ToCommonExchange(this Exchange exchange)
        {
            CommonExchange toreturn = new CommonExchange()
            {
                ExchFee = exchange.ExchFee,
                ExchCode = exchange.ExchCode,
                ExchEnabled = exchange.ExchEnabled,
                ExchTradeEnabled = exchange.ExchTradeEnabled,
                ExchFeeBuy = exchange.ExchFeeBuy,
                ExchFeeSell = exchange.ExchFeeSell,
                ExchName = exchange.ExchName,
            };

            return toreturn;
        }
    }
}
