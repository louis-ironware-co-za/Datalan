using CryptoExchange.Net.Objects;
using Luno.Net.Objects.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Luno.Net.Interfaces.Clients.GeneralApi
{
    public interface ILunoClientGeneralApiGeneral
    {
        Task<WebCallResult<LunoAccountInfo>> GetAccountInfoAsync(long? receiveWindow, CancellationToken ct = default);
        Task<WebCallResult<LunoMarket>> GetExchangeSymbolsAsync(long? receiveWindow, CancellationToken ct = default);
    }
}
