using CoinigyWebsocketClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Coinigy
{
    public class LiveOrderBook
    {
        public List<OrderItem> Buys { get; set; } = new List<OrderItem>();
        public List<OrderItem> Sells { get; set; } = new List<OrderItem>();
    }
}
