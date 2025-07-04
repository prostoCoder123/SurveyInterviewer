using SurveyInterviewer.Abstractions;
using SurveyInterviewer.EfCore;

namespace SurveyInterviewer.Implementations;

public class QuestionAnswerRepository(SurveysDbContext _context) : GenericRepository<QuestionAnswer>(_context), IQuestionAnswerRepository
{
}
