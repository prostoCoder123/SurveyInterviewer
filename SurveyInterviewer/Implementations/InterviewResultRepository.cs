using SurveyInterviewer.Abstractions;
using SurveyInterviewer.EfCore;

namespace SurveyInterviewer.Implementations;

public class InterviewResultRepository(SurveysDbContext _context) : GenericRepository<InterviewResult>(_context), IInterviewResultRepository
{
}