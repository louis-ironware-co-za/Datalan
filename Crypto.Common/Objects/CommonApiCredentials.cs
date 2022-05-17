using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Objects
{
    public class CommonApiCredentials
    {
        public string ApiKey { get; set; } = "";
        public string ApiSecret { get; set; } = "";
        public string SocketKey { get; set; } = "";
        public string SocketSecret { get; set; } = "";

        public CommonApiCredentials()
        {
        }
    }
}
