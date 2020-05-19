using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpiritTime.Core.Contracts;

namespace SpiritTime.Persistence.Repositories
{

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _dbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public Task<List<T>> GetAllIncludeAsync(Expression<Func<T, object>> predicate)
        {
            return _dbSet.Include(predicate).ToListAsync();
        }

        public Task<T> GetUniqueByAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefaultAsync(predicate);
        }
        public Task<T> GetUniqueByIncludeAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> included)
        {
            return _dbSet.Where(predicate).Include(included).FirstOrDefaultAsync();
        }

        public Task<List<T>> GetMultipleByAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToListAsync();
        }
        public Task<List<T>> GetMultipleIncludeAsync(Expression<Func<T, bool>> predicate
        , Expression<Func<T, object>> included)
        {
            return _dbSet
                .Where(predicate)
                .Include(included)
                .ToListAsync();
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<List<object>> SelectMulitpleAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> selection)
        {
            return await _dbSet.Where(predicate).Select(selection).ToListAsync();
        }
        public async Task<object> SelectFromUniqueAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> selection)
        {
            return await _dbSet.Where(predicate).Select(selection).FirstOrDefaultAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
