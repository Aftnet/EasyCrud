using EasyCrud.Factories;
using EasyCrud.Logging;
using EasyCrud.Models;
using EasyCrud.Repositories;

namespace EasyCrud.MVC.Controllers
{
    public abstract class RepositoryController<TRecordType, TIndexType> : RepositoryFactoryController
        where TRecordType : class , IModel<TIndexType>, new()
    {
        protected readonly IRepository<TRecordType, TIndexType> DataAccess;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DataAccess.Dispose();
            }

            base.Dispose(disposing);
        }

        protected RepositoryController(IRepositoryFactory dataAccessFactory, ILogger logger)
            : base(dataAccessFactory, logger)
        {
            DataAccess = RepositoryFactory.GetRepository<TRecordType, TIndexType>();
        }

        protected IRepository<TOtherRecordType, TOtherIndexType> GetContextSharingRepository<TOtherRecordType, TOtherIndexType>() where TOtherRecordType : class , IModel<TOtherIndexType>
        {
            return RepositoryFactory.GetContextSharingRepository<TOtherRecordType, TOtherIndexType, TRecordType, TIndexType>(DataAccess);
        }
    }
}
