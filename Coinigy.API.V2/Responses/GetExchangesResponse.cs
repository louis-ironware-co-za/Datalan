using Coinigy.API.V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.Responses
{
    public class GetExchangesResponse
    {
        public List<Exchange> Result { get; set; } = new List<Exchange>();
    }
}
