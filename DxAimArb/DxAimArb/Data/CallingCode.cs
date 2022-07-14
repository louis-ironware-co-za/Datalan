using DevExpress.Xpo;
namespace DxAimArb.Data
{
    public class CallingCode: XPObject
    {
        public CallingCode(Session session) : base(session) { }

        private string? fCountry;
        [Size(250)]
        public string? Country
        {
            get { return fCountry; }
            set { SetPropertyValue(nameof(Country), ref fCountry, value); }
        }

        private string? fCode;
        [Size(10)]
        public string? Code
        {
            get { return fCode; }
            set { SetPropertyValue(nameof(Code), ref fCode, value); }
        }

        private string? fInternationalCalling;
        [Size(20)]
        public string? InternationalCalling
        {
            get { return fInternationalCalling; }
            set { SetPropertyValue(nameof(InternationalCalling), ref fInternationalCalling, value); }
        }
    }
}
