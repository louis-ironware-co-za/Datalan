using Coinigy.API.V2.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coinigy.API.V2.Responses
{
    public class GetUserBalanceResponse
    {
        public List<UserBalance> Result { get; set; } = new List<UserBalance>();
    }
}
