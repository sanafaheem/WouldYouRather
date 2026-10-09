namespace WouldYouRather.API.Features.Question.Models.DTO;

public class QuestionDto
{
	public string Id { get; set; } = string.Empty;

	public string QuestionDescription { get; set; } = string.Empty;

	public string OptionA { get; set; } = string.Empty;

	public string OptionB { get; set; } = string.Empty;
}
