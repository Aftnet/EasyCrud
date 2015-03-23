using EasyCrud.Factories;
using EasyCrud.Logging;
using EasyCrud.Models;
using EasyCrud.Repositories;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace EasyCrud.WebAPI.Controllers
{
    public abstract class SearchController<TSearchQueryType, TResponseType> : RepositoryFactoryController
        where TSearchQueryType : class, ISearchQuery, new()
    {
        protected abstract int MaxNumElementsToTake { get; }
        protected abstract Task<SearchResponse<TResponseType>> GenerateSearchResponseAsync(TSearchQueryType query);

        protected SearchController(IRepositoryFactory dataAccessFactory, ILogger logger)
            : base(dataAccessFactory, logger)
        {
        }

        // GET: <BaseAddress>/<ControllerName>/<Id>
        public async Task<SearchResponse<TResponseType>> Get([FromUri] TSearchQueryType id)
        {
            SearchResponse<TResponseType> output = null;
            try
            {
                output = await GenerateSearchResponseAsync(id);
            }
            catch (SystemException exception)
            {
                RaiseErrorMessage(HttpStatusCode.InternalServerError, InternalServerErrorMsg, LogSeverity.FatalError, exception);
            }
            return output;
        }

        protected int GetTakeFromQuery(TSearchQueryType query)
        {
            var output = Math.Min(Math.Max(0, query.NumElementsToTake()), MaxNumElementsToTake);
            return output;
        }

        protected int GetSkipFromQuery(TSearchQueryType query)
        {
            var output = Math.Max(0, query.NumElementsToSkip());
            return output;
        }

        protected IQueryable<TResponseType> LimitResultSet(TSearchQueryType query, IQueryable<TResponseType> queryable)
        {
            var take = GetTakeFromQuery(query);
            var skip = GetSkipFromQuery(query);
            queryable = queryable.Take(take).Skip(skip);
            return queryable;
        }

        protected async Task<SearchResponse<TResponseType>> FormatResponseFromResultsQueryable<TRecordType, TIndexType>(IRepository<TRecordType, TIndexType> dataSource, TSearchQueryType query, IQueryable<TResponseType> resultsQueryable)
            where TRecordType : class , IModel<TIndexType>, new()
        {
            var results = await dataSource.ToArrayAsync(LimitResultSet(query, resultsQueryable));
            var totalResults = await dataSource.CountAsync(resultsQueryable);
            var skipped = GetSkipFromQuery(query);

            var output = new SearchResponse<TResponseType>
            {
                Results = results,
                TotalResults = totalResults,
                Skipped = skipped
            };
            return output;
        }
    }
}