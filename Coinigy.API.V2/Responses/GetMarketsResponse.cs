using Coinigy.API.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.Responses
{
    public class GetMarketsResponse
    {
        public List<MarketListEntry> Result { get; set; } = new List<MarketListEntry>();
    }
}
