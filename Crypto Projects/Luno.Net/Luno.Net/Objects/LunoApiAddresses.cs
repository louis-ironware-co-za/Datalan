using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects
{
    public class LunoApiAddresses
    {
        public string RestClientAddress { get; set; } = string.Empty;
        public string SocketClientAddress { get; set; } = string.Empty;

        public static LunoApiAddresses Default = new LunoApiAddresses
        {
            RestClientAddress = "https://api.luno.com",
            SocketClientAddress = "wss://ws.luno.com/api/1/",
        };
    }
}
