using SurveyInterviewer.EfCore;
using SurveyInterviewer.Abstractions;

namespace SurveyInterviewer.Implementations;

public class InterviewResultRepository(SurveysDbContext _context) : GenericRepository<InterviewResult>(_context), IInterviewResultRepository
{
}