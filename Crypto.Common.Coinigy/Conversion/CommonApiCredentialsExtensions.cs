using Crypto.Common.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Coinigy.Conversion
{
    internal static class CommonApiCredentialsExtensions
    {
        internal static CommonApiCredentials Convert(Crypto.Coinigy.ApiCredentials credentials)
        {
            return new CommonApiCredentials()
            {
                ApiKey = credentials.ApiKey,
                ApiSecret = credentials.ApiSecret,
                SocketKey = credentials.SocketKey,
                SocketSecret = credentials.SocketSecret,
            };
        }

        internal static Crypto.Coinigy.ApiCredentials Convert(CommonApiCredentials credentials)
        {
            return new Crypto.Coinigy.ApiCredentials()
            {
                ApiKey = credentials.ApiKey,
                ApiSecret = credentials.ApiSecret,
                SocketKey = credentials?.SocketKey,
                SocketSecret = credentials.SocketSecret,
            };
        }
    }
}
