using SurveyInterviewer.Abstractions;
using SurveyInterviewer.EfCore;

namespace SurveyInterviewer.Implementations;

public class SurveyQuestionRepository(SurveysDbContext _context) : GenericRepository<SurveyQuestion>(_context), ISurveyQuestionRepository
{
}
