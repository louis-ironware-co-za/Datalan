using DevExpress.Xpo;

namespace DxAimArb.Data
{
    public class AspNetUserTokens : XPLiteObject
    {
        public AspNetUserTokens(Session session) : base(session) { }
        string fValue;
        [Size(SizeAttribute.Unlimited)]
        public string Value
        {
            get { return fValue; }
            set { SetPropertyValue<string>(nameof(Value), ref fValue, value); }
        }
        public struct CompoundKey1Struct
        {
            [Size(450)]
            [Persistent("UserId")]
            public string UserId { get; set; }
            [Size(128)]
            [Persistent("LoginProvider")]
            public string LoginProvider { get; set; }
            [Size(128)]
            [Persistent("Name")]
            public string Name { get; set; }
        }
        [Key, Persistent]
        public CompoundKey1Struct CompoundKey1;
    }


}
