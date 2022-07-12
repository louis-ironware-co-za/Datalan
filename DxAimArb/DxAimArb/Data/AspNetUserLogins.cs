using DevExpress.Xpo;

namespace DxAimArb.Data
{
    public class AspNetUserLogins : XPLiteObject
    {
        public AspNetUserLogins(Session session) : base(session) { }
        string fProviderDisplayName;
        [Size(SizeAttribute.Unlimited)]
        public string ProviderDisplayName
        {
            get { return fProviderDisplayName; }
            set { SetPropertyValue<string>(nameof(ProviderDisplayName), ref fProviderDisplayName, value); }
        }
        string fUserId;
        [Indexed(Name = @"IX_AspNetUserLogins_UserId")]
        [Size(450)]
        public string UserId
        {
            get { return fUserId; }
            set { SetPropertyValue<string>(nameof(UserId), ref fUserId, value); }
        }
        public struct CompoundKey1Struct
        {
            [Size(128)]
            [Persistent("LoginProvider")]
            public string LoginProvider { get; set; }
            [Size(128)]
            [Persistent("ProviderKey")]
            public string ProviderKey { get; set; }
        }
        [Key, Persistent]
        public CompoundKey1Struct CompoundKey1;
    }

}
