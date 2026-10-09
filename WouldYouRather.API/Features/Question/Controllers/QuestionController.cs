using Microsoft.AspNetCore.Mvc;
using WouldYouRather.API.Features.Question.Models.DTO;
using WouldYouRather.API.Features.Question.Services;

namespace WouldYouRather.API.Features.Question.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController(IQuestionService questionService) : ControllerBase
{
	[HttpGet("today")]
	public async Task<IActionResult> GetTodayQuestions()
	{
		var question = await questionService.GetQuestionOfTheDayAsync();
		return Ok(question);
	}

	[HttpPost("vote")]
	public async Task<IActionResult> Vote([FromBody] VoteRequestDto voteRequest)
    {
        var result = await questionService.VoteQuestionAsync(voteRequest);
        return Ok(result);
    }
}
