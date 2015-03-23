using EasyCrud.Models;
using EasyCrud.Repositories;

namespace EasyCrud.Factories
{
    public interface IRepositoryFactory
    {
        IRepository<TRecordType, TIndexType> GetRepository<TRecordType, TIndexType>() where TRecordType : class, IModel<TIndexType>;

        IRepository<TRecordType, TIndexType> GetContextSharingRepository<TRecordType, TIndexType, TSourceRecordType, TSourceIndexType>(IRepository<TSourceRecordType, TSourceIndexType> source)
            where TRecordType : class , IModel<TIndexType>
            where TSourceRecordType : class , IModel<TSourceIndexType>;
    }
}
