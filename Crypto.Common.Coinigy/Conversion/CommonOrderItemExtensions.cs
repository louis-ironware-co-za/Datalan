using CoinigyWebsocketClient.Models;
using Crypto.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Coinigy.Conversion
{
    public static class CommonOrderItemExtensions
    {
        public static CommonOrderItem ToCommonOrderItem(this OrderItem item)
        {
            CommonOrderItem commonOrderItem = new CommonOrderItem()
            {
                OrderType = item.Ordertype,
                Price = item.Price,
                Quantity = item.Quantity,
                Total = item.Total,
            };

            return commonOrderItem;
        }
    }
}
