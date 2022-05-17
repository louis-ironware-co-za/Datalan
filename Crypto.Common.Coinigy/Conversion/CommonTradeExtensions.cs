using CoinigyWebsocketClient.Models;
using Crypto.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Coinigy.Conversion
{
    public static class CommonTradeExtensions
    {
        public static CommonTradeItem ToCommonTradeItem(this TradeItem item)
        {
            CommonTradeItem commonTradeItem = new CommonTradeItem()
            {
                Exchange = item.Exchange,
                ExchId = item.ExchId,
                MarketId = item.Marketid,
                Price = item.Price,
                Quantity = (decimal)item.Quantity,
                Type = item.Type,
                TradeId = item.Tradeid,
            };

            return commonTradeItem;
        }
    }
}
