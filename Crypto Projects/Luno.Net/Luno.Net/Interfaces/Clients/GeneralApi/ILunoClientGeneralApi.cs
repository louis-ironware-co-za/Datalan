using System;
using System.Collections.Generic;
using System.Text;

namespace Luno.Net.Interfaces.Clients.GeneralApi
{
    /// <summary>
    /// Luno general endpoints
    /// </summary>
    public interface ILunoClientGeneralApi : IDisposable
    {
        public ILunoClientGeneralApiGeneral General { get; }
    }
}
