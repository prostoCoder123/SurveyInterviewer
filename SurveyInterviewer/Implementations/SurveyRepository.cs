using SurveyInterviewer.EfCore;
using SurveyInterviewer.Abstractions;

namespace SurveyInterviewer.Implementations;

public class SurveyRepository(SurveysDbContext _context) : GenericRepository<Survey>(_context), ISurveyRepository
{
}