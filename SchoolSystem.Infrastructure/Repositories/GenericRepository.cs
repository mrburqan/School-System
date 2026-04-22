using SchoolSystem.Core.Entites;
using SchoolSystem.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SchoolSystem.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly SchoolSystemEntities _context;
        private readonly DbSet<TEntity> _dbSet;
        public GenericRepository(SchoolSystemEntities context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }


        public virtual async Task<bool> AddAsync(TEntity entity)
        {
            if (entity == null)
                return false;

            _dbSet.Add(entity);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return false;

            return await _dbSet.AnyAsync(predicate);
        }

        public virtual async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return null;

            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<(IEnumerable<TEntity> Data, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> filter = null, string orderByColumn = null, bool isAscending = true, bool asNoTraking = false, params Expression<Func<TEntity, object>>[] includes)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (includes != null && includes.Any())
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (!string.IsNullOrEmpty(orderByColumn))
            {
                var parameter = Expression.Parameter(typeof(TEntity), "p");
                var property = Expression.Property(parameter, orderByColumn);
                var lambda = Expression.Lambda(property, parameter);

                var methodName = isAscending ? "OrderBy" : "OrderByDescending";
                query = (IOrderedQueryable<TEntity>)typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(TEntity), property.Type)
                    .Invoke(null, new object[] { query, lambda });
            }
            else
            {
                var keyProperty = typeof(TEntity).GetProperties().First();
                var parameter = Expression.Parameter(typeof(TEntity), "p");
                var property = Expression.Property(parameter, keyProperty.Name);
                var lambda = Expression.Lambda(property, parameter);

                query = ((IOrderedQueryable<TEntity>)typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(TEntity), property.Type)
                    .Invoke(null, new object[] { query, lambda }));
            }



            if (asNoTraking)
            {
                query = query.AsNoTracking();
            }

            var totalCount = await query.CountAsync();
            var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (data, totalCount);
        }

        public async Task<bool> DeleteRange(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return false;

            var entities = await _dbSet.Where(predicate).ToListAsync();
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }

        public async Task<bool> DeleteFirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                return false;

            var entity = await GetFirstOrDefaultAsync(predicate);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            if (entity == null)
                return false;

            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
                entry.State = EntityState.Modified;
            }
            else
            {
                entry.State = EntityState.Modified;
            }
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public virtual async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.AsNoTracking().ToListAsync();
        }

        public SchoolSystemEntities GetContext()
        {
            return _context;
        }
    }
}



