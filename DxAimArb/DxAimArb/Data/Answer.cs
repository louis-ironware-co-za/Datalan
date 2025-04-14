using DevExpress.Xpo;

namespace DxAimArb.Data
{
    [Persistent("Answers")]
    public class Answer : XPObject
    {
        public Answer(Session session) : base(session) { }

        [Association("User-Answers")]
        public User User { get; set; } // The parent user (assigning the questions)

        [Association("SubUser-Answers")]
        public User SubUser { get; set; } // The colleague being rated

        [Association("Question-Answers")]
        public Question Question { get; set; } // The question being answered

        public int Rating { get; set; } // The answer or rating (e.g., 1-5)
    }

}
