


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WouldYouRather.API.Features.Question.Models
{
    public class Question
    {
        // Stored as a native Mongo ObjectId but exposed as string so callers don't need a MongoDB.Bson reference.
        [BsonId]
        [BsonRepresentation(representation: BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("questionDescription")]
        public string QuestionDescription { get; set; }

        [BsonElement("optionA")]
        public string OptionA { get; set; }

        [BsonElement("optionB")]
        public string OptionB { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("voteForA")]
        public int VoteForA { get; set; }

        [BsonElement("voteForB")]
        public int VoteForB { get; set; }

        // yyyy-MM-dd rather than DateTime so "question of the day" lookups are an exact string match, not a UTC day-range query.
         [BsonElement("questionDate")]
        public string QuestionDate { get; set; } = "";
        public Question(string questionDescription, string optionA, string optionB)
        {
            Id = ObjectId.GenerateNewId().ToString();
            QuestionDescription = questionDescription;
            OptionA = optionA;
            OptionB = optionB;
            CreatedAt = DateTime.UtcNow;
            VoteForA = 0;
            VoteForB = 0;
            QuestionDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
        }
    }
}   