using LunoIntegration.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LunoIntegration
{
    public static class MarketInfoMethods
    {
        private const string ResourceBase = "exchange/1/markets";

        public static async Task<MarketsInfo> GetMarketsInfoAsync(this LunoClient lunoClient)
        {
            return await lunoClient.GetAsync2<MarketsInfo>(ResourceBase, null);
        }
    }
}
