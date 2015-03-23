using EasyCrud.Logging;
using EasyCrud.Models;
using EasyCrud.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EasyCrud.EF.Repositories
{
    public abstract class EntityFrameworkRepositoryBase<TRecordType, TIndexType> : RepositoryBase<TRecordType, TIndexType>
        where TRecordType : class , IModel<TIndexType>
    {
        const string DatabaseSaveChangesErrorMsg = "Unable to save changes to database";
        const string DatabaseContextDetachErrorMsg = "Unable to detach object from database context";

        protected abstract DbContext DatabaseCreator();
        protected abstract DbSet<TRecordType> RecordSetSelector(DbContext database);

        private bool _isDbOwner = false;
        private DbContext _database;
        public DbContext Database
        {
            get
            {
                if (_database == null)
                {
                    _database = DatabaseCreator();
                    _isDbOwner = true;
                }
                return _database;
            }
            set
            {
                _recordSet = null;
                if (_isDbOwner && _database != null)
                {
                    _database.Dispose();
                    _isDbOwner = false;
                }
                _database = value;
            }
        }

        private DbSet<TRecordType> _recordSet;
        public DbSet<TRecordType> RecordSet
        {
            get
            {
                if (_recordSet == null)
                {
                    _recordSet = RecordSetSelector(Database);
                }
                return _recordSet;
            }
        }

        protected EntityFrameworkRepositoryBase(ILogger logger) : base(logger)
        {
        }

        public override void Dispose()
        {
            Database = null;
        }

        public override async Task<TIndexType> CreateAsync(TRecordType input)
        {
            input = RecordSet.Add(input);
            await SaveChangesToDbAsync();
            return input.GetKey();
        }

        public override async Task<TRecordType> ReadAsync(TIndexType id)
        {
            TRecordType output = await RecordSet.FindAsync(id);
            if (output != null)
            {
                try
                {
                    ((IObjectContextAdapter)Database).ObjectContext.Detach(output); //Needed to detach result from DB context, otherwise update method will fail and throw exception
                }
                catch(Exception exception)
                {
                    Logger.Log(LogSeverity.FatalError, DatabaseContextDetachErrorMsg, exception);
                    throw;
                }
            }
            return output;
        }

        public override async Task UpdateAsync(TRecordType input)
        {
            RecordSet.Attach(input);
            var inputEntry = Database.Entry(input);
            inputEntry.State = EntityState.Modified;
            await SaveChangesToDbAsync();
        }

        public override async Task DeleteAsync(TIndexType id)
        {
            var feedbackToRemove = await RecordSet.FindAsync(id);
            RecordSet.Remove(feedbackToRemove);
            await SaveChangesToDbAsync();
        }

        #region Linq extension methods substitutes
        public override Task<bool> AllAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.AllAsync(predicate);
            return output;
        }

        public override Task<bool> AnyAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.AnyAsync();
            return output;
        }

        public override Task<bool> AnyAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.AnyAsync(predicate);
            return output;
        }

        public override Task<decimal?> AverageAsync(IQueryable<decimal?> source)
        {
            var output = source.AverageAsync();
            return output;
        }

        public override Task<decimal> AverageAsync(IQueryable<decimal> source)
        {
            var output = source.AverageAsync();
            return output;
        }

        public override Task<double?> AverageAsync(IQueryable<double?> source)
        {
            var output = source.AverageAsync();
            return output;
        }

        public override Task<double> AverageAsync(IQueryable<double> source)
        {
            var output = source.AverageAsync();
            return output;
        }

        public override Task<float?> AverageAsync(IQueryable<float?> source)
        {
            var output = source.AverageAsync();
            return output;
        }

        public override Task<float> AverageAsync(IQueryable<float> source)
        {
            var output = source.AverageAsync();
            return output;
        }

        public override Task<double?> AverageAsync(IQueryable<int?> source)
        {
            var output = source.AverageAsync();
            return output;
        }

        public override Task<double> AverageAsync(IQueryable<int> source)
        {
            var output = source.AverageAsync();
            return output;
        }

        public override Task<double?> AverageAsync(IQueryable<long?> source)
        {
            var output = source.AverageAsync();
            return output;
        }

        public override Task<double> AverageAsync(IQueryable<long> source)
        {
            var output = source.AverageAsync();
            return output;
        }

        public override Task<decimal?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
        {
            var output = source.AverageAsync(selector);
            return output;
        }

        public override Task<decimal> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            var output = source.AverageAsync(selector);
            return output;
        }

        public override Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
        {
            var output = source.AverageAsync(selector);
            return output;
        }

        public override Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double>> selector)
        {
            var output = source.AverageAsync(selector);
            return output;
        }

        public override Task<float?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
        {
            var output = source.AverageAsync(selector);
            return output;
        }

        public override Task<float> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float>> selector)
        {
            var output = source.AverageAsync(selector);
            return output;
        }

        public override Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
        {
            var output = source.AverageAsync(selector);
            return output;
        }

        public override Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int>> selector)
        {
            var output = source.AverageAsync(selector);
            return output;
        }

        public override Task<double?> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
        {
            var output = source.AverageAsync(selector);
            return output;
        }

        public override Task<double> AverageAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long>> selector)
        {
            var output = source.AverageAsync(selector);
            return output;
        }

        public override Task<bool> ContainsAsync<TSource>(IQueryable<TSource> source, TSource item)
        {
            var output = source.ContainsAsync(item);
            return output;
        }

        public override Task<int> CountAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.CountAsync();
            return output;
        }

        public override Task<int> CountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.CountAsync(predicate);
            return output;
        }

        public override Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.FirstAsync();
            return output;
        }

        public override Task<TSource> FirstAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.FirstAsync(predicate);
            return output;
        }

        public override Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.FirstOrDefaultAsync();
            return output;
        }

        public override Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.FirstOrDefaultAsync(predicate);
            return output;
        }

        public override Task<long> LongCountAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.LongCountAsync();
            return output;
        }

        public override Task<long> LongCountAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.LongCountAsync(predicate);
            return output;
        }

        public override Task<TSource> MaxAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.MaxAsync();
            return output;
        }

        public override Task<TResult> MaxAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            var output = source.MaxAsync(selector);
            return output;
        }

        public override Task<TSource> MinAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.MinAsync();
            return output;
        }

        public override Task<TResult> MinAsync<TSource, TResult>(IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            var output = source.MinAsync(selector);
            return output;
        }

        public override Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.SingleAsync();
            return output;
        }

        public override Task<TSource> SingleAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.SingleAsync(predicate);
            return output;
        }

        public override Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.SingleOrDefaultAsync();
            return output;
        }

        public override Task<TSource> SingleOrDefaultAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            var output = source.SingleOrDefaultAsync(predicate);
            return output;
        }

        public override Task<decimal?> SumAsync(IQueryable<decimal?> source)
        {
            var output = source.SumAsync();
            return output;
        }

        public override Task<decimal> SumAsync(IQueryable<decimal> source)
        {
            var output = source.SumAsync();
            return output;
        }

        public override Task<double?> SumAsync(IQueryable<double?> source)
        {
            var output = source.SumAsync();
            return output;
        }

        public override Task<double> SumAsync(IQueryable<double> source)
        {
            var output = source.SumAsync();
            return output;
        }

        public override Task<float?> SumAsync(IQueryable<float?> source)
        {
            var output = source.SumAsync();
            return output;
        }

        public override Task<float> SumAsync(IQueryable<float> source)
        {
            var output = source.SumAsync();
            return output;
        }

        public override Task<int?> SumAsync(IQueryable<int?> source)
        {
            var output = source.SumAsync();
            return output;
        }

        public override Task<int> SumAsync(IQueryable<int> source)
        {
            var output = source.SumAsync();
            return output;
        }

        public override Task<long?> SumAsync(IQueryable<long?> source)
        {
            var output = source.SumAsync();
            return output;
        }

        public override Task<long> SumAsync(IQueryable<long> source)
        {
            var output = source.SumAsync();
            return output;
        }

        public override Task<decimal?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
        {
            var output = source.SumAsync(selector);
            return output;
        }

        public override Task<decimal> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            var output = source.SumAsync(selector);
            return output;
        }

        public override Task<double?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
        {
            var output = source.SumAsync(selector);
            return output;
        }

        public override Task<double> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, double>> selector)
        {
            var output = source.SumAsync(selector);
            return output;
        }

        public override Task<float?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
        {
            var output = source.SumAsync(selector);
            return output;
        }

        public override Task<float> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, float>> selector)
        {
            var output = source.SumAsync(selector);
            return output;
        }

        public override Task<int?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
        {
            var output = source.SumAsync(selector);
            return output;
        }

        public override Task<int> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, int>> selector)
        {
            var output = source.SumAsync(selector);
            return output;
        }

        public override Task<long?> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
        {
            var output = source.SumAsync(selector);
            return output;
        }

        public override Task<long> SumAsync<TSource>(IQueryable<TSource> source, Expression<Func<TSource, long>> selector)
        {
            var output = source.SumAsync(selector);
            return output;
        }

        public override Task<TSource[]> ToArrayAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.ToArrayAsync();
            return output;
        }

        public override Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IQueryable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var output = source.ToDictionaryAsync(keySelector);
            return output;
        }

        public override Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var output = source.ToDictionaryAsync(keySelector, elementSelector);
            return output;
        }

        public override Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            var output = source.ToDictionaryAsync(keySelector, comparer);
            return output;
        }

        public override Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            var output = source.ToDictionaryAsync(keySelector, elementSelector, comparer);
            return output;
        }

        public override Task<List<TSource>> ToListAsync<TSource>(IQueryable<TSource> source)
        {
            var output = source.ToListAsync();
            return output;
        }
        #endregion

        public override void ClearData()
        {
            Database.Database.Delete();
        }

        protected async Task SaveChangesToDbAsync()
        {
            try
            {
                await Database.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                Logger.Log(LogSeverity.FatalError, DatabaseSaveChangesErrorMsg, exception);
                throw;
            }
        }

        public override IQueryable<TRecordType> GetQueryable()
        {
            return RecordSet.AsQueryable();
        }
    }
}
