using Crypto.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Objects
{
    public delegate void OrderBookInitializedDelegate(CommonLiveOrderBook book);
    public delegate void OrderBookUpdatedDelegate(CommonLiveOrderBook book);
    public delegate void OrderBookTradeUpdatedDelegate(CommonLiveOrderBook book);
    public delegate void OrderBookStatusUpdatedDelegate(CommonLiveOrderBook book);
}
