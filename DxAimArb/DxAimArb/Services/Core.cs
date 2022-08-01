using DevExpress.Xpo;
using DxAimArb.Data;
using DxAimArb.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;

using PureWebSockets;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

using DevExpress.Xpo.DB;
using System.Xml.Serialization;



namespace DxAimArb.Services
{
    public class Core
    {


        private readonly string _ApplicationName = "AIM Arbitrage";
        public string ApplicationName
        {
            get { return _ApplicationName; }
        }

        public AspNetUsers AspNetUser { get; set; }

        public string Message { get; private set; } = "";
        public string UserId { get; private set; } = "";
        public string UserIdGuid { get { return UserId.Substring(1);} }
        public string FirstName { get; private set; } = "";
        public string LastName { get; private set; } = "";
        public string Email { get; private set; } = "";
        public bool Enabled { get; private set; } = false;
        public int Counter { get; private set; } = 0;
        public string currencyFormat { get; set; } = "R###,###,###,###,0.00";
        public string xmlFile { get; private set; }
        public string xsdFile { get; private set; }

        public bool IsAdmin()
        {
            switch (UserId)
            {
                case "$c96ea3b7-76c2-4bb4-a21e-d290422b76c7":
                case "$fbe91be6-5163-40cf-9362-5ed8b701fa3":
                    return true;
                default:
                    return false;
            }
        }

        public string latestVer { get; } = "(RC6)";

        public string notallowedhere { get; } = "User is not registered, not logged in or does not have permissions to view this page";

        public void UpdateDataFiles(ComponentBase Source, string xml, string xsd)
        {
            this.xmlFile = xml;
            this.xsdFile = xsd;
            NotifyStateChanged(Source, "Message");
        }

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
