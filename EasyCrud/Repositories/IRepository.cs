using EasyCrud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EasyCrud.Repositories
{
    public interface IRepository<TRecordType, TIndexType> : IDisposable where TRecordType : class , IModel<TIndexType>
    {
        Task<TIndexType> CreateAsync(TRecordType input);
        Task<TRecordType> ReadAsync(TIndexType id);
        Task UpdateAsync(TRecordType input);
        Task DeleteAsync(TIndexType id);

        IQueryable<TRecordType> GetQueryable();

        #region Linq extension methods substitutes
        Task<bool> AllAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
        Task<bool> AnyAsync<TSource>(IQueryable<TSource> source);
        Task<bool> AnyAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
        Task<decimal?> AverageAsync(IQueryable<decimal?> source);
        Task<decimal> AverageAsync(IQueryable<decimal> source);
        Task<double?> AverageAsync(IQueryable<double?> source);
        Task<double> AverageAsync(IQueryable<double> source);
        Task<float?> AverageAsync(IQueryable<float?> source);
        Task<float> AverageAsync(IQueryable<float> source);
        Task<double?> AverageAsync(IQueryable<int?> source);
        Task<double> AverageAsync(IQueryable<int> source);
        Task<double?> AverageAsync(IQueryable<long?> source);
        Task<double> AverageAsync(IQueryable<long> source);
        Task<decimal?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector);
        Task<decimal> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector);
        Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double?>> selector);
        Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double>> selector);
        Task<float?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float?>> selector);
        Task<float> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float>> selector);
        Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int?>> selector);
        Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int>> selector);
        Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long?>> selector);
        Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long>> selector);
        Task<bool> ContainsAsync<TSource>(IQueryable<TSource> source, TSource item);
        Task<int> CountAsync<TSource>(IQueryable<TSource> source);
        Task<int> CountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
        Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source);
        Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
        Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source);
        Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
        Task<long> LongCountAsync<TSource>(IQueryable<TSource> source);
        Task<long> LongCountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
        Task<TSource> MaxAsync<TSource>(IQueryable<TSource> source);
        Task<TResult> MaxAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector);
        Task<TSource> MinAsync<TSource>(IQueryable<TSource> source);
        Task<TResult> MinAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector);
        Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source);
        Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
        Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source);
        Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate);
        Task<decimal?> SumAsync(IQueryable<decimal?> source);
        Task<decimal> SumAsync(IQueryable<decimal> source);
        Task<double?> SumAsync(IQueryable<double?> source);
        Task<double> SumAsync(IQueryable<double> source);
        Task<float?> SumAsync(IQueryable<float?> source);
        Task<float> SumAsync(IQueryable<float> source);
        Task<int?> SumAsync(IQueryable<int?> source);
        Task<int> SumAsync(IQueryable<int> source);
        Task<long?> SumAsync(IQueryable<long?> source);
        Task<long> SumAsync(IQueryable<long> source);
        Task<decimal?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector);
        Task<decimal> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector);
        Task<double?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double?>> selector);
        Task<double> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double>> selector);
        Task<float?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float?>> selector);
        Task<float> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float>> selector);
        Task<int?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int?>> selector);
        Task<int> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int>> selector);
        Task<long?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long?>> selector);
        Task<long> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long>> selector);
        Task<TSource[]> ToArrayAsync<TSource>(IQueryable<TSource> source);
        Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IQueryable<TSource> source, Func<TSource, TKey> keySelector);
        Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector);
        Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer);
        Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer);
        Task<List<TSource>> ToListAsync<TSource>(IQueryable<TSource> source);
        #endregion

        void ClearData();
    }
}