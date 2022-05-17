using Crypto.Common.Models;
using LunoIntegration.StreamModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Luno.Conversion
{
    public static class CommonTradeExtensions
    {
        public static CommonTradeItem ToCommonTradeItem(this TradeUpdate trade)
        {
            CommonTradeItem item = new CommonTradeItem()
            {
                OrderId = trade.OrderId,
                Price = (decimal)(trade.Order?.Price ?? 0D),
                //TODO: Is this correct?
                Quantity = (decimal)trade.Base,
            };

            return item;
        }
    }
}
