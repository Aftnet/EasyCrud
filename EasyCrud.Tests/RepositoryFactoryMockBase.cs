using EasyCrud.Factories;
using EasyCrud.Logging;
using EasyCrud.Repositories;
using System;
using System.Collections.Generic;

namespace EasyCrud.Tests
{
    public abstract class RepositoryFactoryMockBase : RepositoryFactoryBase
    {
        protected Dictionary<Type, object> BackingStore = new Dictionary<Type, object>();

        protected RepositoryFactoryMockBase(ILogger logger) : base(logger)
        {
        }

        public override IRepository<TRecordType, TIndexType> GetRepository<TRecordType, TIndexType>()
        {
            var output = base.GetRepository<TRecordType, TIndexType>() as RepositoryMockBase<TRecordType, TIndexType>;
            if (output == null)
            {
                throw new InvalidOperationException();
            }

            var recordType = typeof(TRecordType);
            if (BackingStore.ContainsKey(recordType) == false)
            {
                var newBackingList = new List<TRecordType>();
                BackingStore.Add(recordType, newBackingList);        
            }

            var backingList = BackingStore[recordType] as List<TRecordType>;
            if (backingList == null)
            {
                throw new InvalidOperationException();
            }

            output.StoreList = backingList;
            return output;
        }

        public override IRepository<TRecordType, TIndexType> GetContextSharingRepository<TRecordType, TIndexType, TSourceRecordType, TSourceIndexType>(IRepository<TSourceRecordType, TSourceIndexType> source)
        {
            return GetRepository<TRecordType, TIndexType>();
        }
    }
}
