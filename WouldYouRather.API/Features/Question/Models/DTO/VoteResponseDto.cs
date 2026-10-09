namespace WouldYouRather.API.Features.Question.Models.DTO;

public class VoteResponseDto
{
    public int VotesA { get; set; }
    public int VotesB { get; set; }
    public int PercentA { get; set; }
    public int PercentB { get; set; }
    public string WittyComment { get; set; } = "";
}
