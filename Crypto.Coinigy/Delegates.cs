using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Coinigy
{
    public delegate void OrderBookUpdatedDelegate(CoinigyLiveOrderBook book);
    public delegate void TradeUpdatedDelegate(CoinigyLiveOrderBook book);
}
