using EasyCrud.Factories;
using EasyCrud.Logging;
using EasyCrud.Models;
using EasyCrud.Repositories;
using System;
using System.Collections.Generic;

namespace EasyCrud.AzureTables.Factories
{
    public abstract class AzureTablesRepositoryFactoryBase : IRepositoryFactory
    {
        protected readonly ILogger Logger;
        protected Lazy<Dictionary<Type, Type>> RepositoryDictionary;
        protected readonly string StorageConnectionString;

        protected AzureTablesRepositoryFactoryBase(ILogger logger, string storageConnectionString)
        {
            Logger = logger;
            StorageConnectionString = storageConnectionString;
            RepositoryDictionary = new Lazy<Dictionary<Type, Type>>(GenerateRepositoryDictionary);
        }

        public abstract Dictionary<Type, Type> GenerateRepositoryDictionary();

        public virtual IRepository<TRecordType, TIndexType> GetRepository<TRecordType, TIndexType>() where TRecordType : class, IModel<TIndexType>
        {
            var outputType = RepositoryDictionary.Value[typeof(TRecordType)];
            var output = Activator.CreateInstance(outputType, new object[] { Logger, StorageConnectionString }) as IRepository<TRecordType, TIndexType>;
            if (output == null)
            {
                throw new InvalidOperationException();
            }
            return output;
        }

        public virtual IRepository<TRecordType, TIndexType> GetContextSharingRepository<TRecordType, TIndexType, TSourceRecordType, TSourceIndexType>(IRepository<TSourceRecordType, TSourceIndexType> source)
            where TRecordType : class , IModel<TIndexType>
            where TSourceRecordType : class , IModel<TSourceIndexType>
        {
            return GetRepository<TRecordType, TIndexType>();
        }
    }
}