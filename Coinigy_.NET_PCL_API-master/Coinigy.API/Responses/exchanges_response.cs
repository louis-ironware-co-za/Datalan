using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coinigy.API.Responses
{
    [JsonObject(MemberSerialization = MemberSerialization.Fields)]
    public class exchanges_response : BaseResponse
    {

        public bool success;
        public string error;
        public List<Exchange> result;

        //public List<Exchange> data;
        //public List<object> notifications;


    }
}