using Microsoft.AspNetCore.Mvc;
using SurveyInterviewer.Abstractions;
using SurveyInterviewer.Dto;

namespace SurveyInterviewer.Controllers;


[ApiController]
[Route("[controller]")]
public class SurveyController(
    ISurveyService surveyService,
    ISurveyRepository sRepository,
    ILogger<SurveyController> logger) : ControllerBase
{

    [HttpPost("start")]
    [ProducesResponseType<SurveyQuestion>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartInterviewAsync(
        [FromQuery] int surveyId,
        [FromQuery] int userId,
        CancellationToken ct = default)
    {
        logger.LogInformation("Start the interview with Id '{Id}', survey Id '{SurveyId}'", userId, surveyId);

        if (await sRepository.GetByIdAsync(surveyId) == null)
        {
            return BadRequest("Анкета с указанным идентификатором не найдена");
        }

        (SurveyQuestion? firstQuestion, IEnumerable<string> errors) =
            await surveyService.StartSurveyInterviewAsync(userId, surveyId);

        return errors.Any() ? BadRequest(errors) : Ok(firstQuestion);
    }


    [HttpPost("answers")]
    [ProducesResponseType<SurveyQuestion>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProceedQuestionAsync(
        [FromBody] AnswersDto answersDto,
        CancellationToken ct = default)
    {
        if (answersDto.AnswerIds.Count() == 0)
        {
            return BadRequest("Нет ответов");
        }

        logger.LogInformation("Proceed the question with Id '{Id}', survey Id '{SurveyId}', answers count '{Count}'",
            answersDto.QuestionId,
            answersDto.SurveyId,
            answersDto.AnswerIds.Count());

        (SurveyQuestion? nextQuestion, IEnumerable<string> errors) =
            await surveyService.ProceedQuestionAsync(answersDto);

        return errors.Any() ? BadRequest(errors) : Ok(nextQuestion);
    }


    [HttpGet("answers/{surveyInterviewId:int:min(0):max(123456789)}")]
    [ProducesResponseType<IAsyncEnumerable<InterviewResult>>(StatusCodes.Status200OK)]
    public IAsyncEnumerable<InterviewResult> GetInterviewResultsAsync(int surveyInterviewId, CancellationToken ct = default)
    {
        return surveyService.GetInterviewResultsAsync(surveyInterviewId);
    }
}
