using ORMPerformance.Data.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EFCore.BulkExtensions;
using System.Threading.Tasks;

namespace ORMPerformance.Data.Repository
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly AppDbContext _context;
        private DbSet<TEntity> _entity;

        public IQueryable<TEntity> Table => _entity;
        public IQueryable<TEntity> TableNoTracking => _entity.AsNoTracking();

        public EfRepository(AppDbContext context)
        {
            _context = context;
            _entity = context.Set<TEntity>();
        }

        public virtual async Task<TEntity> FindByIdAsync(object id)
        {
            return await _entity.FindAsync(id);
        }

        public IQueryable<TEntity> GetByExp(Expression<Func<TEntity, bool>> expression)
        {
            return _entity.Where(expression);
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _entity.Add(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task InsertRangeAsync(IList<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _entity.AddRange(entities);
            await _context.SaveChangesAsync();
        }

        public virtual async Task BulkInsertRangeAsync(IList<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

           await _context.BulkInsertAsync(entities);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _entity.Update(entity);
            await _context.SaveChangesAsync();

        }
        public virtual async Task BulkUpdateRangeAsync(IList<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await _context.BulkUpdateAsync(entities);
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = EntityState.Deleted;
           // _context.Remove(entity);
            await _context.SaveChangesAsync();

        }

        public virtual async Task DeleteAsync(ICollection<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _entity.RemoveRange(entities);
            await _context.SaveChangesAsync();

        }

        public virtual async Task BulkDeleteRangeAsync(IList<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await _context.BulkDeleteAsync(entities);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
