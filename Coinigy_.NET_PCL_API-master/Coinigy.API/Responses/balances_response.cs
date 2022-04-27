using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coinigy.API.Responses
{
    [JsonObject(MemberSerialization = MemberSerialization.Fields)]
    public class balances_response
    {
        public bool success;
        public string error;
        public List<Balance> result;
        
        //public List<object> notifications;
    }
}