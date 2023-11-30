using System.Linq.Expressions;
using CampaignScheduler.Domain.Models;

namespace CampaignScheduler.Domain.Repository.Interfaces
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        IAsyncEnumerable<T> GetAllAsAsyncStream();
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);
        IAsyncEnumerable<T> FindAsAsyncStream(Expression<Func<T, bool>> filter);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(Guid id, T entity);
        Task DeleteAsync(Guid id);
    }
}

