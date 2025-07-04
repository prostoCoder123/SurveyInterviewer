using SurveyInterviewer.Abstractions;
using SurveyInterviewer.EfCore;

namespace SurveyInterviewer.Implementations;

public class SurveyRepository(SurveysDbContext _context) : GenericRepository<Survey>(_context), ISurveyRepository
{
}