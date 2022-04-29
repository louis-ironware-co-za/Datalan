using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LunoIntegration.Models
{
    public class MarketsInfo
    {
        [JsonProperty(PropertyName = "markets")]
        public List<MarketInfo> Markets { get; set; }   
    }
}
