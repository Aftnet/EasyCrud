using EasyCrud.Factories;
using EasyCrud.Logging;
using EasyCrud.Models;
using System.Threading.Tasks;

namespace EasyCrud.WebAPI.Controllers
{
    public abstract class ModelSearchController<TModelSearchType, TRecordType, TIndexType> : SearchController<TModelSearchType, TRecordType>
        where TRecordType : class , IModel<TIndexType>, new()
        where TModelSearchType : class, IModelSearchQuery<TRecordType>, new()
    {
        protected ModelSearchController(IRepositoryFactory dataAccessFactory, ILogger logger) : base(dataAccessFactory, logger)
        {
        }

        protected override async Task<SearchResponse<TRecordType>> GenerateSearchResponseAsync(TModelSearchType query)
        {
            using (var dataAccess = RepositoryFactory.GetRepository<TRecordType, TIndexType>())
            {
                var resultsQueryable = dataAccess.GetQueryable();
                resultsQueryable = query.GenerateSearchQuery(resultsQueryable);
                var output = await FormatResponseFromResultsQueryable(dataAccess, query, resultsQueryable);
                return output;
            }
        }
    }
}