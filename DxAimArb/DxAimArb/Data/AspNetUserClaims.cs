using DevExpress.Xpo;

namespace DxAimArb.Data
{
    public class AspNetUserClaims : XPLiteObject
    {
        public AspNetUserClaims(Session session) : base(session) { }
        int fId;
        [Key(true)]
        public int Id
        {
            get { return fId; }
            set { SetPropertyValue<int>(nameof(Id), ref fId, value); }
        }
        AspNetUsers fUserId;
        [Size(450)]
        [Association(@"AspNetUserClaimsReferencesAspNetUsers")]
        public AspNetUsers UserId
        {
            get { return fUserId; }
            set { SetPropertyValue<AspNetUsers>(nameof(UserId), ref fUserId, value); }
        }
        string fClaimType;
        [Size(SizeAttribute.Unlimited)]
        public string ClaimType
        {
            get { return fClaimType; }
            set { SetPropertyValue<string>(nameof(ClaimType), ref fClaimType, value); }
        }
        string fClaimValue;
        [Size(SizeAttribute.Unlimited)]
        public string ClaimValue
        {
            get { return fClaimValue; }
            set { SetPropertyValue<string>(nameof(ClaimValue), ref fClaimValue, value); }
        }
    }

}
