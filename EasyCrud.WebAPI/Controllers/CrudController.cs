using EasyCrud.Factories;
using EasyCrud.Logging;
using EasyCrud.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace EasyCrud.WebAPI.Controllers
{
    public abstract class CrudController<TRecordExchangeType, TRecordType, TIndexType> : RepositoryController<TRecordType, TIndexType>
        where TRecordType : class , IModel<TIndexType>, new()
        where TRecordExchangeType : class, IModelDataExchange<TRecordType, TIndexType>, new()
    {
        public const string IdNotFoundErrorMsg = "No data found for the specified ID";
        public const string IdAlreadyInUseErrorMsg = "Id specified is already in use";
        public const string InvalidParametersErrorMsg = "Invalid request parameters specified";

        protected virtual Task<bool> CustomCreationValidityCheck(TRecordExchangeType value)
        {
            return Task.FromResult(true);
        }

        protected virtual Task<bool> CustomEditingValidityCheck(TRecordExchangeType value)
        {
            return Task.FromResult(true);
        }

        protected CrudController(IRepositoryFactory dataAccessFactory, ILogger logger) : base(dataAccessFactory, logger)
        {
        }

        protected async Task EnsureReferencedEntityIsPresentAsync<TOtherRecordType, TOtherIndexType>(bool allowNullKey, TOtherIndexType? key, string errorMessage)
            where TOtherRecordType : class , IModel<TOtherIndexType>
            where TOtherIndexType : struct
        {
            if (allowNullKey)
            {
                if (key.HasValue == false) return;
            }

            await EnsureReferencedEntityIsPresentAsync<TOtherRecordType, TOtherIndexType>(allowNullKey, key.Value, errorMessage);
        }

        protected async Task EnsureReferencedEntityIsPresentAsync<TOtherRecordType, TOtherIndexType>(bool allowNullKey, TOtherIndexType key, string errorMessage)
            where TOtherRecordType : class , IModel<TOtherIndexType>
        {
            if (allowNullKey)
            {
                if (key == null) return;
            }

            TOtherRecordType relatedEntity;
            using (var repository = GetContextSharingRepository<TOtherRecordType, TOtherIndexType>())
            {
                relatedEntity = await repository.ReadAsync(key);
            }

            if (relatedEntity == null)
            {
                RaiseErrorMessage(HttpStatusCode.BadRequest, errorMessage, LogSeverity.DebugInfo);
            }
        }

        protected async Task<bool> ValueIsPresentAsync<TPropertyType>(Func<TRecordType, TPropertyType> propertySelector, TPropertyType value)
        {
            var result = false;
            try
            {
                var queryable = DataAccess.GetQueryable();
                //warning: do not convert to LINQ
                // ReSharper disable once LoopCanBeConvertedToQuery
                var itemsList = await DataAccess.ToListAsync(queryable);
                foreach (var item in itemsList)
                {
                    if (!EqualityComparer<TPropertyType>.Default.Equals(propertySelector(item), value))
                        continue;
                    result = true;
                    break;
                }

            }
            catch (SystemException exception)
            {
                RaiseErrorMessage(HttpStatusCode.InternalServerError, InternalServerErrorMsg, LogSeverity.FatalError, exception);
            }
            return result;
        }

        // GET: <BaseAddress>/<ControllerName>/<Id>
        public async Task<TRecordExchangeType> Get(TIndexType id)
        {
            TRecordType data = null;
            try
            {
                data = await DataAccess.ReadAsync(id);
                if (data == null)
                {
                    RaiseErrorMessage(HttpStatusCode.NotFound, IdNotFoundErrorMsg, LogSeverity.DebugInfo);
                }
            }
            catch (SystemException exception)
            {
                RaiseErrorMessage(HttpStatusCode.InternalServerError, InternalServerErrorMsg, LogSeverity.FatalError, exception);
            }

            var output = new TRecordExchangeType();
            output.InitializeFromModel(data);
            return output;
        }

        // POST: <BaseAddress>/<ControllerName>
        public async Task<CreationResponse<TIndexType>> Post([FromBody]TRecordExchangeType value)
        {
            if (value == null)
            {
                RaiseErrorMessage(HttpStatusCode.BadRequest, InvalidParametersErrorMsg, LogSeverity.DebugInfo);
                return null;
            }
            if (value.IsValidForCreation() == false)
            {
                RaiseErrorMessage(HttpStatusCode.BadRequest, InvalidParametersErrorMsg, LogSeverity.DebugInfo);
            }

            try
            {
                await CustomCreationValidityCheck(value);
            }
            catch (SystemException exception)
            {
                RaiseErrorMessage(HttpStatusCode.InternalServerError, InternalServerErrorMsg, LogSeverity.FatalError, exception);
            }

            var recordFromValue = new TRecordType();
            value.UpdateModel(recordFromValue);

            //If the key is not autogenerated, check if an entity with same ID is in database. If so, throw bad request error.
            if (recordFromValue.KeyIsAutogenerated() == false)
            {
                var sameIdAlreadyInUse = true;

                try
                {
                    await Get(recordFromValue.GetKey());
                }
                catch (HttpResponseException exception)
                {
                    if (exception.Response.StatusCode != HttpStatusCode.NotFound)
                    {
                        throw;
                    }
                    sameIdAlreadyInUse = false;
                }

                if (sameIdAlreadyInUse)
                {
                    RaiseErrorMessage(HttpStatusCode.BadRequest, IdAlreadyInUseErrorMsg, LogSeverity.DebugInfo);
                }
            }

            TIndexType recordKey = default(TIndexType);
            try
            {
                recordKey = await DataAccess.CreateAsync(recordFromValue);
            }
            catch (SystemException exception)
            {
                RaiseErrorMessage(HttpStatusCode.InternalServerError, InternalServerErrorMsg, LogSeverity.FatalError, exception);
            }

            recordFromValue.SetKey(recordKey);
            var output = new CreationResponse<TIndexType>(recordKey);
            return output;
        }

        // PUT: <BaseAddress>/<ControllerName>
        public async Task Put([FromBody]TRecordExchangeType value)
        {
            if (value == null)
            {
                RaiseErrorMessage(HttpStatusCode.BadRequest, InvalidParametersErrorMsg, LogSeverity.DebugInfo);
                return;
            }
            if (value.IsValidForEditing() == false)
            {
                RaiseErrorMessage(HttpStatusCode.BadRequest, InvalidParametersErrorMsg, LogSeverity.DebugInfo);
            }

            try
            {
                await CustomEditingValidityCheck(value);
            }
            catch (SystemException exception)
            {
                RaiseErrorMessage(HttpStatusCode.InternalServerError, InternalServerErrorMsg, LogSeverity.FatalError, exception);
            }

            var record = new TRecordType();
            value.UpdateModel(record);
            var recordExchange = await Get(record.GetKey());
            recordExchange.UpdateModel(record);
            value.UpdateModel(record);

            try
            {
                await DataAccess.UpdateAsync(record);

            }
            catch (SystemException exception)
            {
                RaiseErrorMessage(HttpStatusCode.InternalServerError, InternalServerErrorMsg, LogSeverity.FatalError, exception);
            }
        }

        // DELETE: <BaseAddress>/<ControllerName>/<Id>
        public async Task Delete(TIndexType id)
        {
            await Get(id);

            try
            {
                await DataAccess.DeleteAsync(id);
            }
            catch (SystemException exception)
            {
                RaiseErrorMessage(HttpStatusCode.InternalServerError, InternalServerErrorMsg, LogSeverity.FatalError, exception);
            }
        }
    }
}
