namespace WouldYouRather.API.Features.Question.Models.DTO;

public class VoteRequestDto
{
    public string QuestionId { get; set; } = string.Empty;
    public string Option { get; set; }  = string.Empty;
}
