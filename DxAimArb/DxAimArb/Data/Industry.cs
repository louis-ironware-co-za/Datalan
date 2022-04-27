using DevExpress.Xpo;

namespace DxAimArb.Data
{
    public class Industry : XPObject
    {
        public Industry(Session session) : base(session) { }

        private string? fName;
        [Size(SizeAttribute.Unlimited)]
        public string? Name
        {
            get { return fName; }
            set { SetPropertyValue(nameof(Name), ref fName, value); }
        }
    }
}
