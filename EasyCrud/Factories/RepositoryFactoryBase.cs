using EasyCrud.Logging;
using EasyCrud.Models;
using EasyCrud.Repositories;
using System;
using System.Collections.Generic;

namespace EasyCrud.Factories
{
    public abstract class RepositoryFactoryBase : IRepositoryFactory
    {
        protected readonly ILogger Logger;
        protected Lazy<Dictionary<Tuple<Type, Type>, Type>> RepositoryDictionary;

        protected RepositoryFactoryBase(ILogger logger)
        {
            Logger = logger;
            RepositoryDictionary = new Lazy<Dictionary<Tuple<Type, Type>, Type>>(GenerateRepositoryDictionary);
        }

        public abstract Dictionary<Tuple<Type, Type>, Type> GenerateRepositoryDictionary();

        public virtual IRepository<TRecordType, TIndexType> GetRepository<TRecordType, TIndexType>() where TRecordType : class, IModel<TIndexType>
        {
            var key = Tuple.Create(typeof(TRecordType), typeof(TIndexType));
            var outputType = RepositoryDictionary.Value[key];
            var output = Activator.CreateInstance(outputType, new[] {Logger as object}) as IRepository<TRecordType, TIndexType>;
            if(output == null)
            {
                throw new InvalidOperationException();
            }
            return output;
        }

        public abstract IRepository<TRecordType, TIndexType> GetContextSharingRepository<TRecordType, TIndexType, TSourceRecordType, TSourceIndexType>(IRepository<TSourceRecordType, TSourceIndexType> source)
            where TRecordType : class , IModel<TIndexType>
            where TSourceRecordType : class , IModel<TSourceIndexType>;
    }
}
