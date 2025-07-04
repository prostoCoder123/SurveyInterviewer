using System.Text.Json.Serialization;

namespace SurveyInterviewer;

public partial class QuestionAnswer
{
    public int Id { get; set; }

    public int SurveyQuestionId { get; set; }

    public int AnswerId { get; set; }

    public int Order { get; set; }

    public virtual Answer Answer { get; set; } = null!;

    [JsonIgnore]
    public virtual SurveyQuestion SurveyQuestion { get; set; } = null!;

    public virtual ICollection<InterviewResult> InterviewResults { get; set; } = new List<InterviewResult>();

    public virtual TerminatedQuestionAnswer? TerminatedQuestionAnswer { get; set; }
}
