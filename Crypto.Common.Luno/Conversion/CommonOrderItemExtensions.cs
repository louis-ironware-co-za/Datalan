using Crypto.Common.Models;
using LunoIntegration.StreamModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Luno.Conversion
{
    public static class CommonOrderItemExtensions
    {
        public static CommonOrderItem ToCommonOrderItem(this LiveOrder order)
        {
            CommonOrderItem item = new CommonOrderItem()
            {
                //OrderType = To Be populated externally.
                Price = (decimal)order.Price,
                Quantity = (decimal)order.Volume,
                //TODO:How is Total populated?
                Total = (decimal)order.Volume * (decimal)order.Price,
            };

            return item;
        }
    }
}
