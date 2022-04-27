using CryptoExchange.Net.Converters;
using Luno.Net.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Converters
{
    internal class SymbolStatusConverter :BaseConverter<SymbolStatus>
    {
        public SymbolStatusConverter() : this(true)
        {

        }

        public SymbolStatusConverter(bool quotes) : base(quotes)
        {

        }

        protected override List<KeyValuePair<SymbolStatus, string>> Mapping => new List<KeyValuePair<SymbolStatus, string>>
        {
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.PostOnly, "POST_ONLY"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.Suspended, "SUSPENDED"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.Active, "ACTIVE"),

        };
    }
}
