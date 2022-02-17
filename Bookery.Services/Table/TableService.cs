using Microsoft.Azure.Cosmos.Table;

namespace Bookery.Services.Table;

public class TableService<T> : ITableService<T> where T : class, ITableEntity
{
    private readonly CloudTable _table;

    public TableService(string connectionString, string tableName)
    {
        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var tableClient = storageAccount.CreateCloudTableClient();
        _table = tableClient.GetTableReference(tableName);
        _table.CreateIfNotExists();
    }

    public async Task<T?> RetrieveAsync(string partition, string key)
    {
        var operation = TableOperation.Retrieve<T>(partition, key);
        var tableResult = await _table.ExecuteAsync(operation);
        return tableResult.Result as T;
    }

    public async Task<T?> InsertOrMergeAsync(T entity)
    {
        var operation = TableOperation.InsertOrMerge(entity);
        var tableResult = await _table.ExecuteAsync(operation);
        return tableResult.Result as T;
    }

    public async Task<T?> DeleteAsync(T entity)
    {
        var operation = TableOperation.Delete(entity);
        var tableResult = await _table.ExecuteAsync(operation);
        return tableResult.Result as T;
    }
}