using DevExpress.Xpo;

namespace DxAimArb.Data
{
    public class Profile : XPObject
    {
        public Profile(Session session) : base(session) { }

        private string? fTitle;
        [Size(10)]
        public string? Title
        {
            get { return fTitle; }
            set { SetPropertyValue(nameof(Title), ref fTitle, value); }
        }

        private string? fInitials;
        [Size(10)]
        public string? Initials
        {
            get { return fInitials; }
            set { SetPropertyValue(nameof(Initials), ref fInitials, value); }
        }

        private string? fFirstNames;
        [Size(250)]
        public string? FirstNames
        {
            get { return fFirstNames; }
            set { SetPropertyValue(nameof(FirstNames), ref fFirstNames, value); }
        }

        private string? fSurname;
        [Size(250)]
        public string? Surname
        {
            get { return fSurname; }
            set { SetPropertyValue(nameof(Surname), ref fSurname, value); }
        }

        private string? fPreferredName;
        [Size(100)]
        public string? PreferredName
        {
            get { return fPreferredName; }
            set { SetPropertyValue(nameof(PreferredName), ref fPreferredName, value); }
        }

        private string? fGender;
        [Size(10)]
        public string? Gender
        {
            get { return fGender; }
            set { SetPropertyValue(nameof(Gender), ref fGender, value); }
        }

        private DateTime fDateOfBirth;
        public DateTime DateOfBirth
        {
            get { return fDateOfBirth; }
            set { SetPropertyValue(nameof(DateOfBirth), ref fDateOfBirth, value); }
        }

        private string? fCountryOfBirth;
        [Size(250)]
        public string? CountryOfBirth
        {
            get { return fCountryOfBirth; }
            set { SetPropertyValue(nameof(CountryOfBirth), ref fCountryOfBirth, value); }
        }

        private string? fNationality;
        [Size(250)]
        public string? Nationality
        {
            get { return fNationality; }
            set { SetPropertyValue(nameof(Nationality), ref fNationality, value); }
        }

        private string? fIDNumber;
        [Size(13)]
        public string? IDNumber
        {
            get { return fIDNumber; }
            set { SetPropertyValue(nameof(IDNumber), ref fIDNumber, value); }
        }

        private string? fTaxNumber;
        [Size(20)]
        public string? TaxNumber
        {
            get { return fTaxNumber; }
            set { SetPropertyValue(nameof(TaxNumber), ref fTaxNumber, value); }
        }

        private DateTime fSignupDate;
        public DateTime SignUpDate
        {
            get { return fSignupDate; }
            set { SetPropertyValue(nameof(SignUpDate), ref fSignupDate, value); }
        }

        private string fUserId;
        public string UserId
        {
            get { return fUserId; }
            set { SetPropertyValue(nameof(UserId), ref fUserId, value); }
        }

        //CONTACT DETAILS
        private string fEmail;
        [Size(250)]
        public string Email
        {
            get { return fEmail; }
            set { SetPropertyValue(nameof(Email), ref fEmail, value); }
        }

        private string fMobileNumber;
        [Size(50)]
        public string MobilePhoneNumber
        {
            get { return fMobileNumber; }
            set { SetPropertyValue(nameof(MobilePhoneNumber), ref fMobileNumber, value); }
        }

        private string fAddressLine1;
        [Size(250)]
        public string AddressLine1
        {
            get { return fAddressLine1; }
            set { SetPropertyValue(nameof(AddressLine1), ref fAddressLine1, value); }
        }

        private string fAddressLine2;
        [Size(250)]
        public string AddressLine2
        {
            get { return fAddressLine2; }
            set { SetPropertyValue(nameof(AddressLine2), ref fAddressLine2, value); }
        }

        private string fCity;
        [Size(250)]
        public string City
        {
            get { return fCity; }
            set { SetPropertyValue(nameof(City), ref fCity, value); }
        }

        private string fPostalCode;
        [Size(10)]
        public string PostalCode
        {
            get { return fPostalCode; }
            set { SetPropertyValue(nameof(PostalCode), ref fPostalCode, value); }
        }

        private string fProvince;
        [Size(100)]
        public string Province
        {
            get { return fProvince; }
            set { SetPropertyValue(nameof(Province), ref fProvince, value); }
        }

        private string fAddressCity;
        [Size(100)]
        public string AddressCity
        {
            get { return fAddressCity; }
            set { SetPropertyValue(nameof(AddressCity), ref fAddressCity, value); }
        }
        private string fAddressCountry;
        [Size(250)]
        public string AddressCountry
        {
            get { return fAddressCity; }
            set { SetPropertyValue(nameof(AddressCountry), ref fAddressCountry, value); }
        }
        //WORK INFORMATION
        private bool fIsUnemployed;
        public bool IsUnemployed
        {
            get { return fIsUnemployed; }
            set { SetPropertyValue(nameof(IsUnemployed), ref fIsUnemployed, value); }
        }

        private string fEmployerName;
        [Size(250)]
        public string EmployerName
        {
            get { return fEmployerName; }
            set { SetPropertyValue(nameof(EmployerName), ref fEmployerName, value); }
        }

        private string fEmployerIndustry;
        [Size(250)]
        public string EmployerIndustry
        {
            get { return fEmployerIndustry; }
            set { SetPropertyValue(nameof(EmployerIndustry), ref fEmployerIndustry, value); }
        }

        private string fOccupation;
        [Size(100)]
        public string Occupation
        {
            get { return fOccupation; }
            set { SetPropertyValue(nameof(Occupation), ref fOccupation, value); }
        }

        private string fJobTitle;
        [Size(100)]
        public string JobTitle
        {
            get { return fJobTitle; }
            set { SetPropertyValue(nameof(JobTitle), ref fJobTitle, value); }
        }

        private string fMonthlyIncome;
        [Size(100)]
        public string MonthlyIncome
        {
            get { return fMonthlyIncome; }
            set { SetPropertyValue(nameof(MonthlyIncome), ref fMonthlyIncome, value); }
        }

        //FUNDING ACCOUNT
        private string fBankName;
        [Size(100)]
        public string BankName
        {
            get { return fBankName; }
            set { SetPropertyValue(nameof(BankName), ref fBankName, value); }
        }

        private string fAccountNumber;
        [Size(100)]
        public string AccountNumber
        {
            get { return fAccountNumber; }
            set { SetPropertyValue(nameof(AccountNumber), ref fAccountNumber, value); }
        }

        private string fAccountType;
        [Size(100)]
        public string AccountType
        {
            get { return fAccountType; }
            set { SetPropertyValue(nameof(AccountType), ref fAccountType, value); }
        }

        private int fTradeSize;
        public int TradeSize
        {
            get { return fTradeSize; }
            set { SetPropertyValue(nameof(TradeSize), ref fTradeSize, value); }
        }

        private int fAnticipatedVolume;
        public int AnticipatedVolume
        {
            get { return fAnticipatedVolume; }
            set { SetPropertyValue(nameof(AnticipatedVolume), ref fAnticipatedVolume, value); }
        }

        private string fIdDocumentType;
        [Size(100)]
        public string IdDocumentType
        {
            get { return fIdDocumentType; }
            set { SetPropertyValue(nameof(IdDocumentType), ref fIdDocumentType, value); }
        }

        private string fProofOfAddressType;
        [Size(100)]
        public string ProofOfAddressType
        {
            get { return fProofOfAddressType; }
            set { SetPropertyValue(nameof(ProofOfAddressType), ref fProofOfAddressType, value); }
        }

        private string fReferredBy;
        [Size(100)]
        public string ReferredBy
        {
            get { return fReferredBy; }
            set { SetPropertyValue(nameof(ReferredBy), ref fReferredBy, value); }
        }


        public override void AfterConstruction()
        {
            SignUpDate = DateTime.Today;
        }
    }
}
