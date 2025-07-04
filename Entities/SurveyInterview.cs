using System;
using System.Collections.Generic;

namespace SurveyInterviewer;

public partial class SurveyInterview
{
    public int Id { get; set; }

    public int SurveyId { get; set; }

    public int InterviewId { get; set; }

    public virtual Interview Interview { get; set; } = null!;

    public virtual Survey Survey { get; set; } = null!;

    public virtual ICollection<InterviewResult> InterviewResults { get; set; } = new List<InterviewResult>();
}
