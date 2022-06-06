using Crypto.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Requests
{
    public class CommonNewOrderRequest
    {
        public string AuthId { get; set; }

        public string Pair { get; set; }
        public int ExchMktId { get; set; }
        public int Side { get; set; }
        public CommonOrderType Type { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public int BaseAccountId { get; set; }
        public int CounterAccountId { get; set; }
        public int PriceType { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public bool Post_Only { get; set; }

    }
}
