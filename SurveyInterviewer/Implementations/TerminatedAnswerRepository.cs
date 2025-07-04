using SurveyInterviewer.EfCore;
using SurveyInterviewer.Implementations;

namespace SurveyInterviewer.Abstractions;

public class TerminatedAnswerRepository(SurveysDbContext _context) : GenericRepository<TerminatedQuestionAnswer>(_context), ITerminatedAnswerRepository
{
}
