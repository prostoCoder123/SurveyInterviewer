using System.Text.Json.Serialization;

namespace SurveyInterviewer;

public partial class SurveyQuestion
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    public int SurveyId { get; set; }

    public int? PrevQuestionId { get; set; }

    public int? NextQuestionId { get; set; }

    public bool IsMultipleSelection { get; set; }

    [JsonIgnore]
    public virtual ICollection<SurveyQuestion> NextQuestions { get; set; } = new List<SurveyQuestion>();

    [JsonIgnore]
    public virtual ICollection<SurveyQuestion> PrevQuestions { get; set; } = new List<SurveyQuestion>();

    public virtual SurveyQuestion? NextQuestion { get; set; } = null!;

    public virtual SurveyQuestion? PrevQuestion { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;

    public virtual Survey Survey { get; set; } = null!;

    public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();
}
