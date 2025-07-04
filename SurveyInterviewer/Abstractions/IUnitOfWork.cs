using Microsoft.EntityFrameworkCore.Storage;

namespace SurveyInterviewer.Abstractions;

public interface IUnitOfWork
{
    ISurveyQuestionRepository TaskRepository { get; }
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
    Task SaveAsync(CancellationToken ct = default);
    TEntity DetachedClone<TEntity>(TEntity entity) where TEntity : class;
}