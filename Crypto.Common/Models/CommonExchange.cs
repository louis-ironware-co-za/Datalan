using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Models
{
    public class CommonExchange
    {
        public Exchanges ParentExchange { get; set; }
        public decimal ExchFee { get; set; }
        public bool ExchFeeBuy { get; set; }
        public bool ExchFeeSell { get; set; }    
        public string ExchName { get; set; }
        public string ExchCode { get; set; }
        public bool ExchTradeEnabled { get; set; }
        public bool ExchEnabled { get; set; }

    }
}
