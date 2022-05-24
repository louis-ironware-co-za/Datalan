using Coinigy.API.V2.Responses;
using Crypto.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Coinigy.Conversion
{
    public static class CommonUserOrderItemExtensions
    {
        public static CommonUserOrderItem ToCommonUserOrderItem(this UserOrderResponse resp)
        {
            var userOrderItem = new CommonUserOrderItem()
            {
                AuthId = resp.AuthId,
                Market = resp.Market,
                ExchCode = resp.ExchCode,
                Fee = resp.Fee,
                ExchmktId = resp.ExchmktId,
                LimitPrice = resp.LimitPrice,
                OrderId = resp.CoinigyOrderId?.ToString(),
                Quantity = resp.Quantity,
                Side = resp.Side,
                //TODO: confirm that this is correct!
                Type = resp.Side == 0 ? "BUY" : "SELL",
            };

            return userOrderItem;
        }
    }
}
