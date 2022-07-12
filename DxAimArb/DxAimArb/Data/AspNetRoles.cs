using DevExpress.Xpo;

namespace DxAimArb.Data
{
    public partial class AspNetRoles : XPCustomObject
    {
        public AspNetRoles(Session session) : base(session) { }
        string fId;
        [Key]
        [Size(450)]
        public string Id
        {
            get { return fId; }
            set { SetPropertyValue<string>(nameof(Id), ref fId, value); }
        }
        string fName;
        [Size(256)]
        public string Name
        {
            get { return fName; }
            set { SetPropertyValue<string>(nameof(Name), ref fName, value); }
        }
        string fNormalizedName;
        [Indexed(Name = @"RoleNameIndex", Unique = true)]
        [Size(256)]
        public string NormalizedName
        {
            get { return fNormalizedName; }
            set { SetPropertyValue<string>(nameof(NormalizedName), ref fNormalizedName, value); }
        }
        string fConcurrencyStamp;
        [Size(SizeAttribute.Unlimited)]
        public string ConcurrencyStamp
        {
            get { return fConcurrencyStamp; }
            set { SetPropertyValue<string>(nameof(ConcurrencyStamp), ref fConcurrencyStamp, value); }
        }
        [Association(@"AspNetRoleClaimsReferencesAspNetRoles")]
        public XPCollection<AspNetRoleClaims> AspNetRoleClaimsCollection { get { return GetCollection<AspNetRoleClaims>(nameof(AspNetRoleClaimsCollection)); } }
    }
}
