using System;
using System.Collections.Generic;

namespace SurveyInterviewer;

public partial class TerminatedQuestionAnswer
{
    public int QuestionAnswerId { get; set; }

    public string Reason { get; set; } = null!;

    public virtual QuestionAnswer QuestionAnswer { get; set; } = null!;
}
