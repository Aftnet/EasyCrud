using EasyCrud.Factories;
using EasyCrud.Logging;

namespace EasyCrud.MVC.Controllers
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
