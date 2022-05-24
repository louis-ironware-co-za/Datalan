using Crypto.Common.Models;
using LunoIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Luno.Conversion
{
    public static class CommonUserOrderItemExtensions
    {
        public static CommonUserOrderItem ToCommonUserOrderItem(this Order order)
        {
            CommonUserOrderItem item = new CommonUserOrderItem()
            {
                Base = (decimal)order.Base,
                Counter = (decimal)order.Counter,
                FeeBase = (decimal)order.FeeBase,
                FeeQuote = (decimal)order.FeeCounter,
                LimitPrice = (decimal)order.LimitPrice,
                OrderId = order.OrderId,
                Type = order.Type.ToString(),
                Market = order.Pair,
                //TODO: Which quantity is correct?
            };

            return item;
        }
    }
}
