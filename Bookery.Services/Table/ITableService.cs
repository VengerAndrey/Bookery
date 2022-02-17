namespace Bookery.Services.Table;

public interface ITableService<T>
{
    Task<T?> RetrieveAsync(string partition, string key);
    Task<T?> InsertOrMergeAsync(T entity);
    Task<T?> DeleteAsync(T entity);
}