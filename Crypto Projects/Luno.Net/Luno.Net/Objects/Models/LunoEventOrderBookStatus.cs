using Luno.Net.Converters;
using Luno.Net.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects.Models
{
    public class LunoEventOrderBookStatus
    {
        [JsonConverter(typeof(SymbolStatusConverter))]
        public SymbolStatus Status { get; set; }
    }
}
