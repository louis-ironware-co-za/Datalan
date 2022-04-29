using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace LunoIntegration.Enums
{
    public enum TradingStatus
    {
        [EnumMember(Value = "POST_ONLY")]
        Post_Only,
        [EnumMember(Value = "ACTIVE")]
        Active,
        [EnumMember(Value = "SUSPENDED")]
        Suspended,
    }
}
