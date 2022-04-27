using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class LunoEventOrderBookCreate
    {
        public string Order_Id { get; set; }
        /// <summary>
        /// BID or ASK
        /// </summary>
        public string Type { get; set; }
        public decimal? Price { get; set; }
        public decimal? Volume { get; set; }
    }
}
