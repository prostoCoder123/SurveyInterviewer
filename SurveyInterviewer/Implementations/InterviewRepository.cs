using SurveyInterviewer.Abstractions;
using SurveyInterviewer.EfCore;

namespace SurveyInterviewer.Implementations;

public class InterviewRepository(SurveysDbContext _context) : GenericRepository<Interview>(_context), IInterviewRepository
{
}
