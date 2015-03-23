using EasyCrud.Factories;
using EasyCrud.Logging;

namespace EasyCrud.Tests
{
    public abstract class TestBase
    {
        protected ILogger LoggerMock { get; set; }
        public IRepositoryFactory RepositoriesFactory { get; set; }

        protected abstract IRepositoryFactory CreateRepositoriesFactory(ILogger logger);

        public virtual void Initialize()
        {
            LoggerMock = new NullLogger();
            RepositoriesFactory = CreateRepositoriesFactory(LoggerMock);
        }

        public virtual void CleanUp()
        {
        }
    }
}
