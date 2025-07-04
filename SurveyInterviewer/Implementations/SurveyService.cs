using Microsoft.EntityFrameworkCore;
using Npgsql;
using SurveyInterviewer.Abstractions;
using SurveyInterviewer.Dto;
using SurveyInterviewer.EfCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SurveyInterviewer.Implementations;

public class SurveyService(
    ISurveyQuestionRepository sqRepository,
    IInterviewResultRepository irRepository,
    IInterviewRepository iRepository,
    ITerminatedAnswerRepository tRepository,
    IUnitOfWork unitOfWork,
    SurveysDbContext context) : ISurveyService
{
    public IAsyncEnumerable<InterviewResult> GetInterviewResultsAsync(int interviewId, CancellationToken ct = default)
    {
        return irRepository.Get(
            filter: i => i.SurveyInterviewId == interviewId && i.SurveyInterview.Interview.EndedAt != null,
            includeProperties: "QuestionAnswers,QuestionAnswers.Answer")
        .AsNoTracking()
        .AsAsyncEnumerable();
    }

    public async Task<(SurveyQuestion?, IEnumerable<string>)> ProceedQuestionAsync(AnswersDto answersDto, CancellationToken ct = default)
    {
        var data = await sqRepository.Get
        (
            filter: s => s.QuestionId == answersDto.QuestionId && s.SurveyId == answersDto.SurveyId
        )
        .Include(d => d.QuestionAnswers)
        .ThenInclude(q => q.InterviewResults.Where(i => i.SurveyInterviewId == answersDto.InterviewId))
        .Include(d => d.Survey)
        .ThenInclude(f => f.SurveyInterviews.Where(s => s.InterviewId == answersDto.InterviewId))
        .ThenInclude(s => s.Interview)
        .SingleAsync();

        if (data == null)
        {
            return (null, ["Нет данных"]);
        }

        var errors = ValidateSurveyQuestionData(data, answersDto);
        if (errors.Any())
        {
            return (null, errors);
        }

        var surveyInterview = data.Survey.SurveyInterviews.Single(); // where filter
        var interviewResults = answersDto.AnswerIds.Select(
            x => new InterviewResult() { SurveyInterviewId = surveyInterview.Id, QuestionAnswerId = x });

        var isTerminated = await tRepository.CountAsync(
            filter: t => answersDto.AnswerIds.Contains(t.QuestionAnswerId)
        ) > 0;

        (_, IEnumerable<string> errs) = await ExecuteTransactionAsync(interviewResults, async (t, ct) =>
        {
            await irRepository.InsertRangeAsync(interviewResults, ct);
            if (data.NextQuestionId == null || isTerminated)
            {
                surveyInterview.Interview.EndedAt = DateTime.UtcNow;
                surveyInterview.Interview.IsTerminated = isTerminated;
            }
        });

        if (errs.Any())
        {
            return (null, errs);
        }

        SurveyQuestion? nextQuestion = data.NextQuestionId == null
            ? null
            : await sqRepository.Get(filter: s => s.SurveyId == answersDto.SurveyId &&
                                                  s.Id == data.NextQuestionId)
                                .Include(q => q.QuestionAnswers.OrderBy(x => x.Order)).ThenInclude(q => q.Answer)
                                .Include(q => q.Question)
                                .AsNoTracking()
                                .SingleOrDefaultAsync();

        return (nextQuestion, []);

    }

    public async Task<(SurveyQuestion?, IEnumerable<string>)> StartSurveyInterviewAsync(int interviewId, int surveyId, CancellationToken ct = default)
    {
        Interview interview = new Interview()
        {
            StartedAt = DateTime.UtcNow,
            SurveyInterviews = [ new SurveyInterview() { SurveyId = surveyId, InterviewId = interviewId } ]
        };

        (_, IEnumerable<string> errors) = await ExecuteTransactionAsync(interview, async (t, ct) =>
        {
            await iRepository.InsertAsync(t, ct);
        });

        if (errors.Any())
        {
            return (null, errors);
        }

        SurveyQuestion nextQuestion = await sqRepository.Get(
            filter: s => s.SurveyId == surveyId &&
                         s.PrevQuestionId == null && s.NextQuestionId != null)
        .Include(q => q.QuestionAnswers.OrderBy(x => x.Order)).ThenInclude(q => q.Answer)
        .Include(q => q.Question)
        .AsNoTracking()
        .SingleAsync(ct);

        return (nextQuestion, []);
    }

    private async Task<(T? updated, IEnumerable<string> errors)> ExecuteTransactionAsync<T>(
         T entity,
         Action<T, CancellationToken> operation,
         CancellationToken ct = default)
    {
        var errors = new List<string>();

        var strategy = context.Database.CreateExecutionStrategy(); // TODO: ResilientTransaction
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync(ct);
            try
            {
                operation.Invoke(entity, ct);
                await unitOfWork.SaveAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);

                if (ex is DbUpdateException) //TODO: fix autoincrement
                {
                    if ((ex.GetBaseException() as NpgsqlException)?.SqlState == "23505") // Data duplication
                    {
                        errors = ["Duplicated data are not allowed"];
                    }
                }
                else
                {
                    throw;
                }
            }
        });

        return (entity, errors);
    }

    private IEnumerable<string> ValidateSurveyQuestionData(SurveyQuestion data, AnswersDto answerDto)
    {
        ICollection<string> errors = new List<string>();

        if (data.Survey == null)
        {
            return [ "Нет такой анкеты"];
        }

        if (data.Survey.SurveyInterviews.Count() != 1)
        {
            return ["Нет такого интервью"];
        }

        if (answerDto.AnswerIds.Count() > 1 && !data.IsMultipleSelection)
        {
            errors.Add("Возможен только один ответ на данный вопрос");
        }

        if (answerDto.AnswerIds.Except(data.QuestionAnswers.Select(q => q.AnswerId)).Any())
        {
            errors.Add("Неверные варианты ответов");
        }

        var surveyInterview = data.Survey.SurveyInterviews.Single().Interview;
        if (surveyInterview.IsTerminated || surveyInterview.EndedAt != null)
        {
            errors.Add("Нельзя добавить ответ для завершенного интервью");
        }

        // TODO: проверка, что на предыдущий вопрос был добавлен ответ

        return errors;

    }
}