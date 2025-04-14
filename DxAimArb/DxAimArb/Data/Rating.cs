using DevExpress.Xpo;

namespace DxAimArb.Data
{
    [Persistent("Ratings")]
    public class Rating : XPObject
    {
        public Rating(Session session) : base(session) { }

        // The user being rated
        [Association("User-Ratings")]
        public User RatedUser { get; set; }

        // The colleague providing the rating
        public User Colleague { get; set; }

        // The question being rated
        [Association("Question-Ratings")]
        public Question Question { get; set; }

        // The actual rating value
        public int RatingValue { get; set; } // Or use a different data type like double or string if needed
    }

}
