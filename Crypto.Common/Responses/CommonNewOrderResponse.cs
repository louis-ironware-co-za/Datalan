using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Responses
{
    public class CommonNewOrderResponse
    {
        public string ForeignOrderId { get; set; }
        public string OrderId { get; set; }

        public object Tag { get; set; }
    }
}
