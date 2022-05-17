using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Requests
{
    public class CommonCancelOrderRequest
    {
        public string AuthId { get; set; }
        public string OrderId { get; set; }
    }
}
