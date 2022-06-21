namespace DxAimArb.Data
{
    public static class LookupService
    {
        public static readonly IEnumerable<string> Countries = new List<string>()
       {
            "Afghanistan", "Albania", "Algeria", "Andorra", "Angola", "Antigua and Barbuda", "Argentina", "Armenia", "Australia", "Austria", "Azerbaijan", "Bahamas", "Bahrain", "Bangladesh", "Barbados", "Belarus", "Belgium", "Belize", "Benin", "Bhutan", "Bolivia", "Bosnia and Herzegovina", "Botswana", "Brazil", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Côte d'Ivoire", "Cabo Verde", "Cambodia", "Cameroon", "Canada", "Central African Republic", "Chad", "Chile", "China", "Colombia", "Comoros", "Congo (Congo-Brazzaville)", "Costa Rica", "Croatia", "Cuba", "Cyprus", "Czechia (Czech Republic)", "Democratic Republic of the Congo", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt", "El Salvador", "Equatorial Guinea", "Eritrea", "Estonia", "Eswatini (fmr. 'Swaziland')", "Ethiopia", "Fiji", "Finland", "France", "Gabon", "Gambia", "Georgia", "Germany", "Ghana", "Greece", "Grenada", "Guatemala", "Guinea", "Guinea-Bissau", "Guyana", "Haiti", "Holy See", "Honduras", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq", "Ireland", "Israel", "Italy", "Jamaica", "Japan", "Jordan", "Kazakhstan", "Kenya", "Kiribati", "Kuwait", "Kyrgyzstan", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein", "Lithuania", "Luxembourg", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta", "Marshall Islands", "Mauritania", "Mauritius", "Mexico", "Micronesia", "Moldova", "Monaco", "Mongolia", "Montenegro", "Morocco", "Mozambique", "Myanmar (formerly Burma)", "Namibia", "Nauru", "Nepal", "Netherlands", "New Zealand", "Nicaragua", "Niger", "Nigeria", "North Korea", "North Macedonia", "Norway", "Oman", "Pakistan", "Palau", "Palestine State", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Poland", "Portugal", "Qatar", "Romania", "Russia", "Rwanda", "Saint Kitts and Nevis", "Saint Lucia", "Saint Vincent and the Grenadines", "Samoa", "San Marino", "Sao Tome and Principe", "Saudi Arabia", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Solomon Islands", "Somalia", "South Africa", "South Korea", "South Sudan", "Spain", "Sri Lanka", "Sudan", "Suriname", "Sweden", "Switzerland", "Syria", "Tajikistan", "Tanzania", "Thailand", "Timor-Leste", "Togo", "Tonga", "Trinidad and Tobago", "Tunisia", "Turkey", "Turkmenistan", "Tuvalu", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "United States of America", "Uruguay", "Uzbekistan", "Vanuatu", "Venezuela", "Vietnam", "Yemen", "Zambia", "Zimbabwe"
        };
        public static readonly IEnumerable<string> YesNo = new List<string>()
       {
            "Yes", "No"
        };
        public static readonly IEnumerable<string> Titles = new List<string>()
       {
            "Mr", "Mrs", "Miss", "Ms", "Dr", "Admiral", "Air Comm", "Ambassador", "Baron", "Baroness", "Brig & Mrs", "Brig Gen", "Brigadier", "Brother", "Canon", "Capt", "Chief", "Cllr", "Col", "Commander", "Commander & Mrs", "Consul", "Consul General", "Count", "Countess", "Countess of", "Cpl", "Dame", "Deputy", "Dr & Mrs", "Drs", "Duchess", "Duke", "Earl", "Father", "General", "Gräfin", "HE", "HMA", "Her Grace", "His Excellency", "Ing", "Judge", "Justice", "Lady", "Lic", "Llc", "Lord", "Lord & Lady", "Lt", "Lt Col", "Lt Cpl", "M", "Madam", "Madame", "Major", "Major General", "Marchioness", "Marquis", "Minister", "Mme", "Mr & Dr", "Mr & Mrs", "Mr & Ms", "Prince", "Princess", "Professor", "Prof", "Prof & Dr", "Prof & Mrs", "Prof & Rev", "Prof Dame", "Prof Dr", "Pvt", "Rabbi", "Rear Admiral", "Rev", "Rev & Mrs", "Rev Canon", "Rev Dr", "Senator", "Sgt", "Sheriff", "Sir", "Sir & Lady", "Sister", "Sqr. Leader", "The Earl of", "The Hon", "The Hon Dr", "The Hon Lady", "The Hon Lord", "The Hon Mrs", "The Hon Sir", "The Honourable", "The Rt Hon", "The Rt Hon Dr", "The Rt Hon Lord", "The Rt Hon Sir", "The Rt Hon Visc", "Viscount"
        };
        public static readonly IEnumerable<string> Genders = new List<string>()
       {
            "Male", "Female"
        };

        public static readonly IEnumerable<string> Income = new List<string>()
       {
            "R5,000-R9,999", "R10,000-R19,999", "R20,000-R49,999", "R50,000-R99,999", "R100,000+"
        };

        public static readonly IEnumerable<string> Banks = new List<string>()
       {
            "Absa Bank", "Bidvest Bank", "Capitec Bank", "Discovery Bank", "First National Bank", "Investec Bank", "Mercantile Bank", "Nedbank", "Rand Merchant Bank", "Sasfin Bank", "Standard Bank", "Tyme Bank"
        };

        public static readonly IEnumerable<string> BankAccountTypes = new List<string>()
       {
            "Cheque", "Savings"
        };

        //IDDocumentTypes
        public static readonly IEnumerable<string> IDDocumentTypes = new List<string>()
       {
            "SA ID Book", "SA ID Card (Front and Back)", "SA Passport (If SA ID Book or Card is stolen/lost)"
        };

        public static readonly IEnumerable<string> POADocumentTypes = new List<string>()
       {
            "Recent Utility Bill (Not older than three months)",
            "Bank Statement (Not older than three months)",
            "Municipal Rates and Taxes (Not older than three months)",
            "Levy Statement (Not older than three months)",
            "Morgage Statement (Not older than three months)",
            "Cellular or Telephone Invoice (Not older than three months)",
            "Investment Statement (Not older than three months)",
            "Payslip or Salary Advice (Not older than three months)",
            "Retail or Credit Account Statement (Not older than three months)",
            "Letter from Municipal Representative (Not older than six months)",
            "Official SARS Document (Not older than twelve months)",
            "Insurance Policy (Not older than twelve months)",
            "Government Issued Document (Not older than twelve months)",
            "Registration Document: Educational Institution (Not older than twelve months)",
            "Medical Aid Statement/Policy (Not older than twelve months)",
            "Current Lease/Rental Agreement (Not Expired)",
            "Current Television License (Not Expired)",
            "Current Motor Vehicle License (Not Expired)"
        };
    }
}
