using Coinigy.API.V2.Models;
using Crypto.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Conversion
{
    public static class CommonMarketExtensions
    {
        public static CommonMarket ToCommonMarket(this MarketListEntry entry)
        {
            CommonMarket market = new CommonMarket()
            {
                BaseCurrency = entry.BaseCurrCode,
                ExchCode = entry.ExchCode,
                ExchmktId = entry.ExchmktId,
                MarketIdInt = entry.MarketId,
                QuoteCurrency = entry.QuoteCurrCode,
                MarketName = entry.MarketName,
            };

            return market;
        }

        public static CommonMarket ToCommonMarket(this PublicMarketListEntry entry)
        {
            CommonMarket market = new CommonMarket()
            {
                BaseCurrency = entry.BaseCurrencyCode,
                QuoteCurrency = entry.QuoteCurrencyCode,
                ExchCode = entry.ExchCode,
                ExchmktId = entry.ExchmktId,
                MarketIdInt = entry.MarketId.GetValueOrDefault(),
                MarketName = entry.MarketName,
            };

            return market;
        }

        public static MarketListEntry ToMarketListEntry(this CommonMarket market)
        {
            MarketListEntry marketListEntry = new MarketListEntry()
            {
                BaseCurrCode = market.BaseCurrency,
                ExchCode = market.ExchCode,
                ExchmktId = market.ExchmktId ?? 0,
                MarketId = market.MarketIdInt,
                QuoteCurrCode = market.QuoteCurrency,
            };

            return marketListEntry;
        }
    }
}
