using Microsoft.EntityFrameworkCore.Storage;
using SurveyInterviewer.Abstractions;
using SurveyInterviewer.EfCore;

namespace SurveyInterviewer.Implementations;

public class UnitOfWork(
  SurveysDbContext context,
  ISurveyQuestionRepository taskRepository) : IUnitOfWork, IDisposable
{
    private readonly SurveysDbContext dbContext = context;

    private bool disposed;
    public ISurveyQuestionRepository TaskRepository { get; } = taskRepository;

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken _ct = default) => dbContext.Database.BeginTransactionAsync(_ct);

    public async Task SaveAsync(CancellationToken _ct = default) => _ = await dbContext.SaveChangesAsync(_ct);

    public TEntity DetachedClone<TEntity>(TEntity _entity) where TEntity : class => dbContext.Entry(_entity).CurrentValues.Clone().ToObject() as TEntity ?? default!;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool _disposing)
    {
        if (!disposed)
        {
            if (_disposing)
            {
                dbContext.Dispose();
            }
        }

        disposed = true;
    }
}