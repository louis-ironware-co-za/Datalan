using Luno.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Objects.Models
{
    public class LunoAccountInfo
    {
        public IEnumerable<LunoBalance> Balances { get; set; } = Array.Empty<LunoBalance>();
    }

    public class LunoBalance : ILunoBalance
    {
        public int AccountId {get;set;}
        public string Asset { get; set; } = String.Empty;
        public decimal Available {get;set; }
        public decimal Locked {get;set;}
        public decimal Unconfirmed {get;set;}
    }
}
