using Crypto.Common.Models;
using LunoIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Luno.Conversion
{
    public static class CommonAccountBalanceExtensions
    {
        public static CommonAccountBalance ToCommonAccountBalance(this AccountBalance acc)
        {
            CommonAccountBalance result = new CommonAccountBalance()
            {
                AuthId = acc.AccountId,
                AuthNickname = acc.Name,
                BalanceCurrCode = acc.Asset,
                BalanceAmountHeld = acc.Reserved,
                BalanceAmountAvailable = acc.Balance,
                Tag = acc,
            };

            return result;
        }
    }
}
