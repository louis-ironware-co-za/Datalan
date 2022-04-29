using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Objects
{
    public class ApiCredentials
    {
        public string ApiKey { get; set; } = "";
        public string ApiSecret { get; set; } = "";

        public ApiCredentials(string key, string secret)
        {
            ApiKey = key;
            ApiSecret = secret;
        }
    }
}
