using DevExpress.Xpo;

namespace DxAimArb.Data
{
    [Persistent("Users")]
    public class User : XPObject
    {
        public User(Session session) : base(session) { }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Cellphone { get; set; }
        public string Email { get; set; }

        public int UniqueQuestionsCount
        {
            get
            {
                // Filter the ratings by this user's ID and ensure only distinct questions are counted
                return Ratings
                    .Where(r => r.RatedUser == this) // Ensure the user matches
                    .Select(r => r.Question)        // Select questions
                    .Distinct()                     // Remove duplicates
                    .Count();                       // Count unique questions
            }
        }



        // Direct many-to-many relationship: Colleagues
        [Association("User-Colleagues")]
        public XPCollection<User> Colleagues => GetCollection<User>(nameof(Colleagues));

        // Reciprocal property to complete the association
        [Association("User-Colleagues")]
        public XPCollection<User> LinkedUsers => GetCollection<User>(nameof(LinkedUsers));

        // One-to-Many Relationship: Questions
        [Association("User-Questions")]
        public XPCollection<Question> Questions => GetCollection<Question>(nameof(Questions));

        // Relationship with Ratings table
        [Association("User-Ratings")]
        public XPCollection<Rating> Ratings => GetCollection<Rating>(nameof(Ratings));

        public int ColleaguesCount => Colleagues.Count;
        public int LinkedUsersCount => LinkedUsers.Count;

    }

}
