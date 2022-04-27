using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Interfaces
{
    public interface ILunoBalance
    {
        int AccountId { get; set; }
        string Asset { get; set; }
        /// <summary>
        /// The amount available to send or trade.
        /// </summary>
        [JsonProperty("balance")]
        decimal Available { get; set; }
        /// <summary>
        /// Amount locked by Luno and cannot be sent or traded. This could be due to open orders.
        /// </summary>
        [JsonProperty("reserved")]
        decimal Locked { get; set; }
        /// <summary>
        /// Amount that is awaiting some sort of verification to be credited to this account. 
        /// This could be an on-chain transaction that Luno is waiting for further block verifications to happen.
        /// </summary>
        decimal Unconfirmed { get; set; }
        

    }
}
