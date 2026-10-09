namespace WouldYouRather.API.Features.Question.Services;

public interface IQuestionContentGenerator
{
    Task<GeneratedQuestionContent> GenerateQuestionAsync();

    Task<string> GenerateWittyCommentAsync(
        string questionDescription,
        string optionA,
        int percentA,
        string optionB,
        int percentB);
}

public sealed record GeneratedQuestionContent(string Description, string OptionA, string OptionB);
