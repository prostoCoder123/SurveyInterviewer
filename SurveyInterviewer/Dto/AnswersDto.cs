using System.ComponentModel.DataAnnotations;

namespace SurveyInterviewer.Dto;

public class AnswersDto
{
    [Required]
    [Range(0, int.MaxValue)]
    public int InterviewId { get; init; }


    [Required]
    [Range(0, int.MaxValue)]
    public int SurveyId { get; init; }


    [Required]
    [Range(0, int.MaxValue)]
    public int QuestionId { get; init; }

    public IEnumerable<int> AnswerIds { get; init; } = Enumerable.Empty<int>();
}
