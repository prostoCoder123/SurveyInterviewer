namespace SurveyInterviewer.Dto;

public class QuestionAnswersDto
{
    public SurveyQuestion Question { get; set; } = default!;

    public IEnumerable<InterviewResult> Answers { get; set; } = Enumerable.Empty<InterviewResult>();
}
