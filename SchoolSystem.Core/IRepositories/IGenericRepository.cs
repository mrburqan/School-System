using SchoolSystem.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SchoolSystem.Core.IRepositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<bool> AddAsync(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> GetQueryable();
        SchoolSystemEntities GetContext();

        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null);

        Task<(IEnumerable<TEntity> Data, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<TEntity, bool>> filter = null,
            string orderByColumn = null,
            bool isAscending = true,
            bool asNoTraking = false,
            params Expression<Func<TEntity, object>>[] includes);

        Task<bool> DeleteRange(Expression<Func<TEntity, bool>> predicate);

        Task<bool> DeleteFirstOrDefault(Expression<Func<TEntity, bool>> predicate);
    }
}