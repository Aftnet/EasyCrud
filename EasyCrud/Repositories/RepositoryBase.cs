using EasyCrud.Logging;
using EasyCrud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EasyCrud.Repositories
{
    public abstract class RepositoryBase<TRecordType, TIndexType> : IRepository<TRecordType, TIndexType> where TRecordType : class , IModel<TIndexType>
    {
        protected readonly ILogger Logger;

        protected RepositoryBase(ILogger logger)
        {
            Logger = logger;
        }

        public abstract Task<TIndexType> CreateAsync(TRecordType input);
        public abstract Task<TRecordType> ReadAsync(TIndexType id);
        public abstract Task UpdateAsync(TRecordType input);
        public abstract Task DeleteAsync(TIndexType id);

        public virtual IQueryable<TRecordType> GetQueryable()
        {
            return new TRecordType[0].AsQueryable();
        }

        #region Linq extension methods substitutes
        public virtual Task<bool> AllAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.All(predicate);
            return Task.FromResult(output);
        }

        public virtual Task<bool> AnyAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.Any();
            return Task.FromResult(output);
        }

        public virtual Task<bool> AnyAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.Any(predicate);
            return Task.FromResult(output);
        }

        public virtual Task<decimal?> AverageAsync(IQueryable<decimal?> source)
        {
            var output = source.Average();
            return Task.FromResult(output);
        }

        public virtual Task<decimal> AverageAsync(IQueryable<decimal> source)
        {
            var output = source.Average();
            return Task.FromResult(output);
        }

        public virtual Task<double?> AverageAsync(IQueryable<double?> source)
        {
            var output = source.Average();
            return Task.FromResult(output);
        }

        public virtual Task<double> AverageAsync(IQueryable<double> source)
        {
            var output = source.Average();
            return Task.FromResult(output);
        }

        public virtual Task<float?> AverageAsync(IQueryable<float?> source)
        {
            var output = source.Average();
            return Task.FromResult(output);
        }

        public virtual Task<float> AverageAsync(IQueryable<float> source)
        {
            var output = source.Average();
            return Task.FromResult(output);
        }

        public virtual Task<double?> AverageAsync(IQueryable<int?> source)
        {
            var output = source.Average();
            return Task.FromResult(output);
        }

        public virtual Task<double> AverageAsync(IQueryable<int> source)
        {
            var output = source.Average();
            return Task.FromResult(output);
        }

        public virtual Task<double?> AverageAsync(IQueryable<long?> source)
        {
            var output = source.Average();
            return Task.FromResult(output);
        }

        public virtual Task<double> AverageAsync(IQueryable<long> source)
        {
            var output = source.Average();
            return Task.FromResult(output);
        }

        public virtual Task<decimal?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
        {
            var output = source.Average(selector);
            return Task.FromResult(output);
        }

        public virtual Task<decimal> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            var output = source.Average(selector);
            return Task.FromResult(output);
        }

        public virtual Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
        {
            var output = source.Average(selector);
            return Task.FromResult(output);
        }

        public virtual Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double>> selector)
        {
            var output = source.Average(selector);
            return Task.FromResult(output);
        }

        public virtual Task<float?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
        {
            var output = source.Average(selector);
            return Task.FromResult(output);
        }

        public virtual Task<float> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float>> selector)
        {
            var output = source.Average(selector);
            return Task.FromResult(output);
        }

        public virtual Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
        {
            var output = source.Average(selector);
            return Task.FromResult(output);
        }

        public virtual Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int>> selector)
        {
            var output = source.Average(selector);
            return Task.FromResult(output);
        }

        public virtual Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
        {
            var output = source.Average(selector);
            return Task.FromResult(output);
        }

        public virtual Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long>> selector)
        {
            var output = source.Average(selector);
            return Task.FromResult(output);
        }

        public virtual Task<bool> ContainsAsync<TSource>(IQueryable<TSource> source, TSource item)
        {
            var output = source.Contains(item);
            return Task.FromResult(output);
        }

        public virtual Task<int> CountAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.Count();
            return Task.FromResult(output);
        }

        public virtual Task<int> CountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.Count(predicate);
            return Task.FromResult(output);
        }

        public virtual Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.First();
            return Task.FromResult(output);
        }

        public virtual Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.First(predicate);
            return Task.FromResult(output);
        }

        public virtual Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.FirstOrDefault();
            return Task.FromResult(output);
        }

        public virtual Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.FirstOrDefault(predicate);
            return Task.FromResult(output);
        }

        public virtual Task<long> LongCountAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.LongCount();
            return Task.FromResult(output);
        }

        public virtual Task<long> LongCountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.LongCount(predicate);
            return Task.FromResult(output);
        }

        public virtual Task<TSource> MaxAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.Max();
            return Task.FromResult(output);
        }

        public virtual Task<TResult> MaxAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            var output = source.Max(selector);
            return Task.FromResult(output);
        }

        public virtual Task<TSource> MinAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.Min();
            return Task.FromResult(output);
        }

        public virtual Task<TResult> MinAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            var output = source.Min(selector);
            return Task.FromResult(output);
        }

        public virtual Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.Single();
            return Task.FromResult(output);
        }

        public virtual Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.Single(predicate);
            return Task.FromResult(output);
        }

        public virtual Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.SingleOrDefault();
            return Task.FromResult(output);
        }

        public virtual Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.SingleOrDefault(predicate);
            return Task.FromResult(output);
        }

        public virtual Task<decimal?> SumAsync(IQueryable<decimal?> source)
        {
            var output = source.Sum();
            return Task.FromResult(output);
        }

        public virtual Task<decimal> SumAsync(IQueryable<decimal> source)
        {
            var output = source.Sum();
            return Task.FromResult(output);
        }

        public virtual Task<double?> SumAsync(IQueryable<double?> source)
        {
            var output = source.Sum();
            return Task.FromResult(output);
        }

        public virtual Task<double> SumAsync(IQueryable<double> source)
        {
            var output = source.Sum();
            return Task.FromResult(output);
        }

        public virtual Task<float?> SumAsync(IQueryable<float?> source)
        {
            var output = source.Sum();
            return Task.FromResult(output);
        }

        public virtual Task<float> SumAsync(IQueryable<float> source)
        {
            var output = source.Sum();
            return Task.FromResult(output);
        }

        public virtual Task<int?> SumAsync(IQueryable<int?> source)
        {
            var output = source.Sum();
            return Task.FromResult(output);
        }

        public virtual Task<int> SumAsync(IQueryable<int> source)
        {
            var output = source.Sum();
            return Task.FromResult(output);
        }

        public virtual Task<long?> SumAsync(IQueryable<long?> source)
        {
            var output = source.Sum();
            return Task.FromResult(output);
        }

        public virtual Task<long> SumAsync(IQueryable<long> source)
        {
            var output = source.Sum();
            return Task.FromResult(output);
        }

        public virtual Task<decimal?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
        {
            var output = source.Sum(selector);
            return Task.FromResult(output);
        }

        public virtual Task<decimal> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            var output = source.Sum(selector);
            return Task.FromResult(output);
        }

        public virtual Task<double?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
        {
            var output = source.Sum(selector);
            return Task.FromResult(output);
        }

        public virtual Task<double> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double>> selector)
        {
            var output = source.Sum(selector);
            return Task.FromResult(output);
        }

        public virtual Task<float?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
        {
            var output = source.Sum(selector);
            return Task.FromResult(output);
        }

        public virtual Task<float> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float>> selector)
        {
            var output = source.Sum(selector);
            return Task.FromResult(output);
        }

        public virtual Task<int?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
        {
            var output = source.Sum(selector);
            return Task.FromResult(output);
        }

        public virtual Task<int> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int>> selector)
        {
            var output = source.Sum(selector);
            return Task.FromResult(output);
        }

        public virtual Task<long?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
        {
            var output = source.Sum(selector);
            return Task.FromResult(output);
        }

        public virtual Task<long> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long>> selector)
        {
            var output = source.Sum(selector);
            return Task.FromResult(output);
        }

        public virtual Task<TSource[]> ToArrayAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.ToArray();
            return Task.FromResult(output);
        }

        public virtual Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IQueryable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var output = source.ToDictionary(keySelector);
            return Task.FromResult(output);
        }

        public virtual Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var output = source.ToDictionary(keySelector, elementSelector);
            return Task.FromResult(output);
        }

        public virtual Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var output = source.ToDictionary(keySelector, comparer);
            return Task.FromResult(output);
        }

        public virtual Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            var output = source.ToDictionary(keySelector, elementSelector, comparer);
            return Task.FromResult(output);
        }

        public virtual Task<List<TSource>> ToListAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.ToList();
            return Task.FromResult(output);
        }
        #endregion

        public abstract void ClearData();
        public abstract void Dispose();
    }
}