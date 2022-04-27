using CryptoExchange.Net.Interfaces;
using Luno.Net.Interfaces.Clients.GeneralApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Interfaces.Clients
{
    public interface ILunoClient : IRestClient
    {
        /// <summary>
        /// General API endpoints
        /// </summary>
        ILunoClientGeneralApi GeneralApi { get; }
    }
}
