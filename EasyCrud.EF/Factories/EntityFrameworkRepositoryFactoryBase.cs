using EasyCrud.EF.Repositories;
using EasyCrud.Factories;
using EasyCrud.Logging;
using EasyCrud.Repositories;
using System;

namespace EasyCrud.EF.Factories
{
    public abstract class EntityFrameworkRepositoryFactoryBase : RepositoryFactoryBase
    {
        protected EntityFrameworkRepositoryFactoryBase(ILogger logger) : base(logger)
        {
        }

        public override IRepository<TRecordType, TIndexType> GetContextSharingRepository<TRecordType, TIndexType, TSourceRecordType, TSourceIndexType>(IRepository<TSourceRecordType, TSourceIndexType> source)
        {
            var output = GetRepository<TRecordType, TIndexType>();
            var sourceAsEfRepository = source as EntityFrameworkRepositoryBase<TSourceRecordType, TSourceIndexType>;
            if (sourceAsEfRepository == null)
            {
                throw new ArgumentException();
            }

            var outputAsEfRepository = output as EntityFrameworkRepositoryBase<TRecordType, TIndexType>;
            if (outputAsEfRepository == null)
            {
                throw new InvalidOperationException();
            }

            outputAsEfRepository.Database = sourceAsEfRepository.Database;
            return outputAsEfRepository;
        }
    }
}
