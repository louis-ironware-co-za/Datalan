using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Enums
{
    public enum SymbolStatus
    {
        /// <summary>
        /// Trading is indefinitely suspended. This state is commonly used when new markets are being launched to give traders enough time to setup their orders before trading begins. 
        /// When in this status, orders can only be posted as post-only.
        /// </summary>
        PostOnly,
        /// <summary>
        /// Trading is fully enabled.
        /// </summary>
        Active,
        /// <summary>
        /// Trading has been temporarily suspended due to very high volatility. When in this status, orders can only be posted as post-only.
        /// </summary>
        Suspended,
    }
}
