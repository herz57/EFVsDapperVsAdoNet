using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ORMPerformance.Data.Repository
{
    public interface IRepository<TEntity>
    {
        Task<TEntity> FindByIdAsync(object id);
        IQueryable<TEntity> GetByExp(Expression<Func<TEntity, bool>> expression);
        Task InsertAsync(TEntity entity);
        Task InsertRangeAsync(IList<TEntity> entities);
        Task BulkInsertRangeAsync(IList<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        Task BulkUpdateRangeAsync(IList<TEntity> entities);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(ICollection<TEntity> entities);
        Task BulkDeleteRangeAsync(IList<TEntity> entities);
        Task SaveChangesAsync();

        IQueryable<TEntity> Table { get; }
        IQueryable<TEntity> TableNoTracking { get; }
    }
}
