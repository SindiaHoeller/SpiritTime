using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SpiritTime.Core.Contracts
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetMultipleByAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetUniqueByAsync(Expression<Func<T, bool>> predicate);
        Task<bool> ExistAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        Task<T> GetUniqueByIncludeAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> included);
        Task<List<T>> GetMultipleIncludeAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> included);

        Task<List<T>> GetAllIncludeAsync(Expression<Func<T, object>> predicate);

        Task<object> SelectFromUniqueAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> selection);

        Task<List<object>> SelectMulitpleAsync(Expression<Func<T, bool>> predicate,
            Expression<Func<T, object>> selection);
    }
}
