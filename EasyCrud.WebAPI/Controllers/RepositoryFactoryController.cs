using EasyCrud.Factories;
using EasyCrud.Logging;

namespace EasyCrud.WebAPI.Controllers
{
    public abstract class RepositoryFactoryController : ControllerBase
    {
        protected readonly IRepositoryFactory RepositoryFactory;

        protected RepositoryFactoryController(IRepositoryFactory dataAccessFactory, ILogger logger)
            : base(logger)
        {
            RepositoryFactory = dataAccessFactory;
        }
    }
}
