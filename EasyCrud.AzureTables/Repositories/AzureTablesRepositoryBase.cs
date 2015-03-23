using EasyCrud.AzureTables.Models;
using EasyCrud.Logging;
using EasyCrud.Models;
using EasyCrud.Repositories;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using System.Threading;
using System.Threading.Tasks;

namespace EasyCrud.AzureTables.Repositories
{
    public abstract class AzureTablesRepositoryBase<TRecordType> : RepositoryBase<TRecordType, ApiTableEntityKey>
        where TRecordType : ApiTableEntity, IModel<ApiTableEntityKey>
    {
        protected readonly string StorageConnectionString;
        protected readonly string TableName;

        protected CloudStorageAccount _storageAccount;
        protected CloudStorageAccount StorageAccount
        {
            get
            {
                if(_storageAccount == null)
                {
                    _storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
                }
                return _storageAccount;
            }
        }

        protected CloudTableClient _tableClient;
        protected CloudTableClient TableClient
        {
            get
            {
                if (_tableClient == null)
                {
                    _tableClient = StorageAccount.CreateCloudTableClient();
                }
                return _tableClient;
            }
        }

        protected CloudTable _table;
        protected CloudTable Table
        {
            get
            {
                if (_table == null)
                {
                    _table = TableClient.GetTableReference(TableName);
                    CreateTableIfNotExist();
                }
                return _table;
            }
        }

        protected AzureTablesRepositoryBase(ILogger logger, string storageConnectionString, string tableName)
            : base(logger)
        {
            StorageConnectionString = storageConnectionString;
            TableName = tableName;
        }

        public override async Task<ApiTableEntityKey> CreateAsync(TRecordType input)
        {
            var tableOperation = TableOperation.Insert(input);
            var result = await Table.ExecuteAsync(tableOperation);
            var output = input.GetKey();
            return output;
        }

        public override async Task<TRecordType> ReadAsync(ApiTableEntityKey id)
        {
            var tableOperation = TableOperation.Retrieve<TRecordType>(id.PartitionKey, id.RowKey);
            var result = await Table.ExecuteAsync(tableOperation);
            var output = (TRecordType)result.Result;
            return output;
        }

        public override async Task UpdateAsync(TRecordType input)
        {
            var key = input.GetKey();
            var targetEntity = await ReadAsync(key);
            var tableOperation = TableOperation.Replace(input);
            var result = await Table.ExecuteAsync(tableOperation);
        }

        public override async Task DeleteAsync(ApiTableEntityKey id)
        {
            var targetEntity = await ReadAsync(id);
            var tableOperation = TableOperation.Delete(targetEntity);
            var result = await Table.ExecuteAsync(tableOperation);
        }

        public override void ClearData()
        {
            Table.DeleteIfExists();
            _table = null;
        }

        public override void Dispose()
        {
        }

        private void CreateTableIfNotExist()
        {
            const int sleepTimeMs = 10000;
            do
            {
                try
                {
                    _table.CreateIfNotExists();
                    break;
                }
                catch (StorageException e)
                {
                    if ((e.RequestInformation.HttpStatusCode == 409) && (e.RequestInformation.ExtendedErrorInformation.ErrorCode.Equals(TableErrorCodeStrings.TableBeingDeleted)))
                        Thread.Sleep(sleepTimeMs);// The table is currently being deleted. Try again until it works.
                    else
                        throw;
                }
            } while (true);
        }
    }
}
