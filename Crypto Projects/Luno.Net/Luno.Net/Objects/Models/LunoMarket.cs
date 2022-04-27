using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects.Models
{
    public class LunoMarket
    {
        public IEnumerable<LunoSymbol> Markets { get; set; } = Array.Empty<LunoSymbol>();
    }
}
