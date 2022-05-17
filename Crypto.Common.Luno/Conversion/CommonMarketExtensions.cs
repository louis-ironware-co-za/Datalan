using Crypto.Common.Models;
using LunoIntegration.Enums;
using LunoIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Luno.Conversion
{
    public static  class CommonMarketExtensions
    {
        public static CommonMarket ToCommonMarket(this MarketInfo market)
        {
            return new CommonMarket()
            {
                BaseCurrency = market.BaseCurrency,
                MarketIdString = market.MarketId,
                QuoteCurrency = market.CounterCurrency,
                Status = market.TradingStatus.ToString(),
                MarketName = market.MarketId,
            };
        }

        public static MarketInfo ToMarketInfo(this CommonMarket market)
        {
            MarketInfo marketInfo = new MarketInfo()
            {
                BaseCurrency = market.BaseCurrency,
                CounterCurrency = market.QuoteCurrency,
                MarketId = market.MarketIdString,
            };

            if (Enum.TryParse(market.Status, out TradingStatus stats))
            {
                marketInfo.TradingStatus = stats;
            }

            return marketInfo;
        }
    }
}
