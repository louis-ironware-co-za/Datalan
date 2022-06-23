using DevExpress.Xpo;

namespace DxAimArb.Data
{
    public class Person : XPObject
    {
        public Person(Session session) : base(session) { }

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
            get { return fFirstNames; }
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
        [Size(100)]
        public string? Gender
        {
            get { return fGender; }
            set { SetPropertyValue(nameof(Gender), ref fGender, value); }
        }
    }
}
