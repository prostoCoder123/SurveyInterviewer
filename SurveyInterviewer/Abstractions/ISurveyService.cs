using SurveyInterviewer.Dto;

namespace SurveyInterviewer.Abstractions;

public interface ISurveyService
{
    /// <summary>
    /// Стартует интервью для юзера
    /// </summary>
    /// <param name="interviewId"> ИД юзера </param>
    /// <param name="surveyId"> ИД анкеты </param>
    /// <returns> Первый вопрос интерью </returns>
    Task<(SurveyQuestion?, IEnumerable<string>)> StartSurveyInterviewAsync(int interviewId, int surveyId, CancellationToken ct = default);

    /// <summary>
    /// Отправить ответы на вопрос и перейти к следующему вопросу
    /// </summary>
    /// <param name="question"> вопрос на который отвечали  </param>
    /// <param name="answers"> ответы на вопрос </param>
    /// <returns> следущий вопрос или null </returns>
    Task<(SurveyQuestion?, IEnumerable<string>)> ProceedQuestionAsync(AnswersDto answersDto, CancellationToken ct = default);

    /// <summary>
    /// Получить все результаты опроса
    /// </summary>
    /// <param name="interviewId"></param>
    /// <returns></returns>
    IAsyncEnumerable<InterviewResult> GetInterviewResultsAsync(int interviewId, CancellationToken ct = default);
}