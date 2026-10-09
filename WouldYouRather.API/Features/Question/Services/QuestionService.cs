using MongoDB.Driver;
using WouldYouRather.API.Features.Question.Models.DTO;
using QuestionModel = WouldYouRather.API.Features.Question.Models.Question;

namespace WouldYouRather.API.Features.Question.Services;

public sealed class QuestionService(
    IMongoCollection<QuestionModel> questions,
    IQuestionContentGenerator contentGenerator) : IQuestionService
{
    public async Task<QuestionDto?> GetQuestionOfTheDayAsync()
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var question = await questions.Find(q => q.QuestionDate == today).FirstOrDefaultAsync();

        if (question is null)
        {
            var generated = await contentGenerator.GenerateQuestionAsync();
            question = new QuestionModel(generated.Description, generated.OptionA, generated.OptionB);
            await questions.InsertOneAsync(question);
        }

        return new QuestionDto
        {
            Id = question.Id,
            QuestionDescription = question.QuestionDescription,
            OptionA = question.OptionA,
            OptionB = question.OptionB
        };
    }

    public async Task<VoteResponseDto> VoteQuestionAsync(VoteRequestDto voteRequest)
    {
        UpdateDefinition<QuestionModel> update = voteRequest.Option switch
        {
            "A" => Builders<QuestionModel>.Update.Inc(q => q.VoteForA, 1),
            "B" => Builders<QuestionModel>.Update.Inc(q => q.VoteForB, 1),
            _ => throw new ArgumentException($"Invalid option '{voteRequest.Option}'. Expected 'A' or 'B'.", nameof(voteRequest))
        };

        var question = await questions.FindOneAndUpdateAsync(
            q => q.Id == voteRequest.QuestionId,
            update,
            new FindOneAndUpdateOptions<QuestionModel> { ReturnDocument = ReturnDocument.After });

        if (question is null)
        {
            throw new InvalidOperationException($"Question '{voteRequest.QuestionId}' was not found.");
        }

        var totalVotes = question.VoteForA + question.VoteForB;
        var percentA = totalVotes == 0 ? 0 : (int)Math.Round(question.VoteForA * 100.0 / totalVotes);
        var percentB = totalVotes == 0 ? 0 : (int)Math.Round(question.VoteForB * 100.0 / totalVotes);

        var wittyComment = await contentGenerator.GenerateWittyCommentAsync(
            question.QuestionDescription, question.OptionA, percentA, question.OptionB, percentB);

        return new VoteResponseDto
        {
            VotesA = question.VoteForA,
            VotesB = question.VoteForB,
            PercentA = percentA,
            PercentB = percentB,
            WittyComment = wittyComment
        };
    }
}
