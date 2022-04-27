using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects.Models
{
    /// <summary>
    /// Remove the order from the order book with a given id
    /// </summary>
    public class LunoEventOrderBookDelete
    {
        public string? Order_Id { get; set; }
    }
}
