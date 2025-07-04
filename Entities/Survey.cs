using System;
using System.Collections.Generic;

namespace SurveyInterviewer;

public partial class Survey
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<SurveyInterview> SurveyInterviews { get; set; } = new List<SurveyInterview>();

    public virtual ICollection<SurveyQuestion> SurveyQuestions { get; set; } = new List<SurveyQuestion>();
}
