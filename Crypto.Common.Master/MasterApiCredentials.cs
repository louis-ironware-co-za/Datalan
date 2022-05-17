using Crypto.Common.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.Common.Master
{
    public class MasterApiCredentials : CommonApiCredentials
    {
        public Exchanges Exchange { get; set; }
    }
}
