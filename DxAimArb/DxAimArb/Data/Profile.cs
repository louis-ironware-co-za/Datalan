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

        //private decimal fFreight;
        //public decimal Freight
        //{
        //    get { return fFreight; }
        //    set { SetPropertyValue(nameof(Freight), ref fFreight, value); }
        //}
        //private Customer fCustomer;
        //[Association("CustomerOrders")]
        //public Customer Customer
        //{
        //    get { return fCustomer; }
        //    set { SetPropertyValue(nameof(Customer), ref fCustomer, value); }
        //}
        public override void AfterConstruction()
        {
            SignUpDate = DateTime.Today;
        }
    }
}
