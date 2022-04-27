using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DxAimArb.Services
{
    public class Core
    {

        private readonly string _ApplicationName = "AIM Arbitrage";
        public string ApplicationName
        {
            get { return _ApplicationName; }
        }

        public string Message { get; private set; } = "";
        public string UserId { get; private set; } = "";
        public string FirstName { get; private set; } = "";
        public string LastName { get; private set; } = "";
        public string Email { get; private set; } = "";
        public bool Enabled { get; private set; } = false;
        public int Counter { get; private set; } = 0;

        public void UpdateMessage(ComponentBase Source, string Message)
        {
            this.Message = Message;
            NotifyStateChanged(Source, "Message");
        }
        public void UpdateUser(ComponentBase Source, string UserId, string FirstName, string LastName, string Email)
        {
            this.UserId = UserId;
            NotifyStateChanged(Source, "UserId");
            this.FirstName = FirstName;
            NotifyStateChanged(Source, "FirstName");
            this.LastName = LastName;
            NotifyStateChanged(Source, "LastName");
            this.Email = Email;
            NotifyStateChanged(Source, "Email");
        }

        public void UpdateEnabled(ComponentBase Source, bool Enabled)
        {
            this.Enabled = Enabled;
            NotifyStateChanged(Source, "Enabled");
        }

        public void UpdateCounter(ComponentBase Source, int Counter)
        {
            this.Counter = Counter;
            NotifyStateChanged(Source, "Counter");
        }

        public event Action<ComponentBase, string> StateChanged;

        private void NotifyStateChanged(ComponentBase Source, string Property) => StateChanged?.Invoke(Source, Property);
    }
}
