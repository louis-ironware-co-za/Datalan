using DevExpress.Xpo;

namespace DxAimArb.Data
{
    public class AspNetUsers : XPLiteObject
    {
        public AspNetUsers(Session session) : base(session) { }

        string fId;
        [Key]
        [Size(450)]
        public string Id
        {
            get { return fId; }
            set { SetPropertyValue<string>(nameof(Id), ref fId, value); }
        }
        string fUserName;
        [Size(256)]
        public string UserName
        {
            get { return fUserName; }
            set { SetPropertyValue<string>(nameof(UserName), ref fUserName, value); }
        }
        string fNormalizedUserName;
        [Indexed(Name = @"UserNameIndex", Unique = true)]
        [Size(256)]
        public string NormalizedUserName
        {
            get { return fNormalizedUserName; }
            set { SetPropertyValue<string>(nameof(NormalizedUserName), ref fNormalizedUserName, value); }
        }
        string fEmail;
        [Size(256)]
        public string Email
        {
            get { return fEmail; }
            set { SetPropertyValue<string>(nameof(Email), ref fEmail, value); }
        }
        string fNormalizedEmail;
        [Indexed(Name = @"EmailIndex")]
        [Size(256)]
        public string NormalizedEmail
        {
            get { return fNormalizedEmail; }
            set { SetPropertyValue<string>(nameof(NormalizedEmail), ref fNormalizedEmail, value); }
        }
        bool fEmailConfirmed;
        public bool EmailConfirmed
        {
            get { return fEmailConfirmed; }
            set { SetPropertyValue<bool>(nameof(EmailConfirmed), ref fEmailConfirmed, value); }
        }
        string fPasswordHash;
        [Size(SizeAttribute.Unlimited)]
        public string PasswordHash
        {
            get { return fPasswordHash; }
            set { SetPropertyValue<string>(nameof(PasswordHash), ref fPasswordHash, value); }
        }
        string fSecurityStamp;
        [Size(SizeAttribute.Unlimited)]
        public string SecurityStamp
        {
            get { return fSecurityStamp; }
            set { SetPropertyValue<string>(nameof(SecurityStamp), ref fSecurityStamp, value); }
        }
        string fConcurrencyStamp;
        [Size(SizeAttribute.Unlimited)]
        public string ConcurrencyStamp
        {
            get { return fConcurrencyStamp; }
            set { SetPropertyValue<string>(nameof(ConcurrencyStamp), ref fConcurrencyStamp, value); }
        }
        string fPhoneNumber;
        [Size(SizeAttribute.Unlimited)]
        public string PhoneNumber
        {
            get { return fPhoneNumber; }
            set { SetPropertyValue<string>(nameof(PhoneNumber), ref fPhoneNumber, value); }
        }
        bool fPhoneNumberConfirmed;
        public bool PhoneNumberConfirmed
        {
            get { return fPhoneNumberConfirmed; }
            set { SetPropertyValue<bool>(nameof(PhoneNumberConfirmed), ref fPhoneNumberConfirmed, value); }
        }
        bool fTwoFactorEnabled;
        public bool TwoFactorEnabled
        {
            get { return fTwoFactorEnabled; }
            set { SetPropertyValue<bool>(nameof(TwoFactorEnabled), ref fTwoFactorEnabled, value); }
        }
        bool fLockoutEnabled;
        public bool LockoutEnabled
        {
            get { return fLockoutEnabled; }
            set { SetPropertyValue<bool>(nameof(LockoutEnabled), ref fLockoutEnabled, value); }
        }
        int fAccessFailedCount;
        public int AccessFailedCount
        {
            get { return fAccessFailedCount; }
            set { SetPropertyValue<int>(nameof(AccessFailedCount), ref fAccessFailedCount, value); }
        }
        string ffirstName;
        [Size(250)]
        [ColumnDbDefaultValue("(N'')")]
        public string firstName
        {
            get { return ffirstName; }
            set { SetPropertyValue<string>(nameof(firstName), ref ffirstName, value); }
        }
        string flastName;
        [Size(250)]
        [ColumnDbDefaultValue("(N'')")]
        public string lastName
        {
            get { return flastName; }
            set { SetPropertyValue<string>(nameof(lastName), ref flastName, value); }
        }
        int fProfileID;
        public int ProfileID
        {
            get { return fProfileID; }
            set { SetPropertyValue<int>(nameof(ProfileID), ref fProfileID, value); }
        }
        [Association(@"AspNetUserClaimsReferencesAspNetUsers")]
        public XPCollection<AspNetUserClaims> AspNetUserClaimsCollection { get { return GetCollection<AspNetUserClaims>(nameof(AspNetUserClaimsCollection)); } }
   
        public string Claims
        {
            get
            {
                var a = AspNetUserClaimsCollection.Select(o => o.ClaimValue).Distinct();
                return string.Join(",", a);
            }
        }

    }
}
