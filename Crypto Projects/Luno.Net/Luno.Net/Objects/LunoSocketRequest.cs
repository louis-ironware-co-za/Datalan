using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects
{
    internal class LunoSocketRequest
    {
        [JsonProperty("api_key_id")]
        public string ApiKey { get; set; }
        [JsonProperty("api_key_secret")]
        public string ApiSecret { get; set; }
    }
    //internal class LunoSocketRequest
    //{
    //    [JsonProperty("method")]
    //    public string Method { get; set; }
    //    [JsonProperty("params")]
    //    public string[] Params { get; set; } = Array.Empty<string>();
    //    [JsonProperty("id")]
    //    public int Id { get; set; }
    //}
}
