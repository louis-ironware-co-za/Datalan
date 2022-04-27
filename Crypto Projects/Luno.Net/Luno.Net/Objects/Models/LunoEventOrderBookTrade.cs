using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects.Models
{
    public class LunoEventOrderBookTrade
    {
        public decimal Base { get; set; }
        public decimal Counter { get; set; }
        public string? MarkerOrderId { get; set; }
        public string? TakerOrderId { get; set; }
    }
}
