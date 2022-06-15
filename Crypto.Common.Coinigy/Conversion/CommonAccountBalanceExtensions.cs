using Coinigy.API.V2.Models;
using Crypto.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Coinigy.Conversion
{
    public static class CommonAccountBalanceExtensions
    {
        public static CommonAccountBalance ToCommonAccountBalance(this UserBalance balance)
        {
            CommonAccountBalance commonAccountBalance = new CommonAccountBalance()
            {
                Tag = balance,
                BalanceAmountAvailable = balance.BalanceAmountAvailable,
                BalanceAmountHeld = balance.BalanceAmountHeld,
                BalanceCurrCode = balance.BalanceCurrCode,
                BalanceQuoteCurrCode = balance.BalanceQuoteCurrCode,
            };

            return commonAccountBalance;
        }
    }
}
