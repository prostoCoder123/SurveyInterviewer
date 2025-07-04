using System;
using System.Collections.Generic;

namespace SurveyInterviewer;

public partial class InterviewResult
{
    public int SurveyInterviewId { get; set; }
    public int QuestionAnswerId { get; set; }

    public virtual SurveyInterview SurveyInterview { get; set; } = null!;

    public virtual QuestionAnswer QuestionAnswer { get; set; } = null!;

    public virtual ICollection<SurveyInterview> SurveyInterviews { get; set; } = new List<SurveyInterview>();
    public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; } = new List<QuestionAnswer>();
}
