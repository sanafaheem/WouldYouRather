using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Google;

namespace WouldYouRather.API.Features.Question.Services;

public sealed class QuestionContentGenerator(Kernel kernel) : IQuestionContentGenerator
{
    private const string DefaultWittyComment = "Tough crowd! The votes are in either way.";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    // Belt-and-suspenders with the prompt instructions below: these are enforced by Gemini's own
    // classifiers server-side, so content is blocked even if a prompt-injection attempt talks the
    // model into ignoring its instructions. BlockLowAndAbove is the strictest available threshold.
    // DangerousContent covers violent/dangerous content under Google's current harm taxonomy;
    // "Violence" is a legacy PaLM-era category this connector still exposes but the live API rejects.
    private static readonly IList<GeminiSafetySetting> SafetySettings = new List<GeminiSafetySetting>
    {
        new(GeminiSafetyCategory.Harassment, GeminiSafetyThreshold.BlockLowAndAbove),
        new(GeminiSafetyCategory.SexuallyExplicit, GeminiSafetyThreshold.BlockLowAndAbove),
        new(GeminiSafetyCategory.DangerousContent, GeminiSafetyThreshold.BlockLowAndAbove),
    };

    public async Task<GeneratedQuestionContent> GenerateQuestionAsync()
    {
        const string prompt = """
            You are generating content for a "Would You Rather" game aimed at a general, family-friendly
            audience that includes children.
            Come up with one creative, fun would-you-rather question with two distinct options.
            The content must be strictly kid-appropriate: no violence, no sexual or romantic content,
            no profanity, no references to drugs or alcohol, and nothing scary or disturbing.
            Respond with ONLY valid JSON, no markdown fences, no extra text, in exactly this shape:
            {"question": "...", "optionA": "...", "optionB": "..."}
            """;

        var response = await kernel.InvokePromptAsync<string>(prompt, CreateSafeArguments());
        var json = StripMarkdownFence(response ?? string.Empty);

        GeneratedQuestionPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<GeneratedQuestionPayload>(json, JsonOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Gemini returned an unparsable question payload: {response}", ex);
        }

        if (payload is null ||
            string.IsNullOrWhiteSpace(payload.Question) ||
            string.IsNullOrWhiteSpace(payload.OptionA) ||
            string.IsNullOrWhiteSpace(payload.OptionB))
        {
            throw new InvalidOperationException($"Gemini returned an incomplete question payload: {response}");
        }

        return new GeneratedQuestionContent(payload.Question, payload.OptionA, payload.OptionB);
    }

    public async Task<string> GenerateWittyCommentAsync(
        string questionDescription,
        string optionA,
        int percentA,
        string optionB,
        int percentB)
    {
        var prompt = $"""
            The "Would You Rather" question was: "{questionDescription}"
            Option A: "{optionA}" got {percentA}% of votes.
            Option B: "{optionB}" got {percentB}% of votes.
            Write ONE short, witty, playful comment (max 20 words) reacting to this result.
            This is for a general, family-friendly audience that includes children: keep it strictly
            kid-appropriate, with no violence, no sexual or romantic references, and no profanity.
            Respond with only the comment text, no quotes, no markdown.
            """;

        try
        {
            var response = await kernel.InvokePromptAsync<string>(prompt, CreateSafeArguments());
            return string.IsNullOrWhiteSpace(response) ? DefaultWittyComment : response.Trim();
        }
        catch (Exception)
        {
            // Gemini being unavailable (or the response being blocked by safety filters)
            // shouldn't fail a vote that already succeeded in Mongo.
            return DefaultWittyComment;
        }
    }

    private static KernelArguments CreateSafeArguments() =>
        new(new GeminiPromptExecutionSettings { SafetySettings = SafetySettings });

    private static string StripMarkdownFence(string text)
    {
        var trimmed = text.Trim();
        if (!trimmed.StartsWith("```", StringComparison.Ordinal))
        {
            return trimmed;
        }

        var firstNewline = trimmed.IndexOf('\n');
        trimmed = firstNewline >= 0 ? trimmed[(firstNewline + 1)..] : trimmed;

        var fenceEnd = trimmed.LastIndexOf("```", StringComparison.Ordinal);
        if (fenceEnd >= 0)
        {
            trimmed = trimmed[..fenceEnd];
        }

        return trimmed.Trim();
    }

    private sealed class GeneratedQuestionPayload
    {
        public string Question { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
    }
}
