namespace SurveyInterviewer;

public partial class Interview
{
    public int Id { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public bool IsTerminated { get; set; }

    public virtual ICollection<SurveyInterview> SurveyInterviews { get; set; } = new List<SurveyInterview>();
}
