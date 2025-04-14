using DevExpress.Xpo;

namespace DxAimArb.Data
{
    [Persistent("Questions")]
    public class Question : XPObject
    {
        public Question(Session session) : base(session) { }

        public string QuestionText { get; set; }

        // Reference to the associated user
        [Association("User-Questions")]
        public User AssignedUser { get; set; }

        // Relationship with Ratings table
        [Association("Question-Ratings")]
        public XPCollection<Rating> Ratings => GetCollection<Rating>(nameof(Ratings));
    }

}
