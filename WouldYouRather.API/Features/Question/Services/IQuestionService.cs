using WouldYouRather.API.Features.Question.Models.DTO;

namespace WouldYouRather.API.Features.Question.Services;

public interface IQuestionService
{
	Task<QuestionDto?> GetQuestionOfTheDayAsync();

	Task<VoteResponseDto> VoteQuestionAsync(VoteRequestDto voteRequest);
}
