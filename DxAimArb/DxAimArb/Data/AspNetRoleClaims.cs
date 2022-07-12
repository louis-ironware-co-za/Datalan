using DevExpress.Xpo;

namespace DxAimArb.Data
{
    public class AspNetRoleClaims : XPLiteObject
    {
        public AspNetRoleClaims(Session session) : base(session) { }
        int fId;
        [Key(true)]
        public int Id
        {
            get { return fId; }
            set { SetPropertyValue<int>(nameof(Id), ref fId, value); }
        }
        AspNetRoles fRoleId;
        [Size(450)]
        [Association(@"AspNetRoleClaimsReferencesAspNetRoles")]
        public AspNetRoles RoleId
        {
            get { return fRoleId; }
            set { SetPropertyValue<AspNetRoles>(nameof(RoleId), ref fRoleId, value); }
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
