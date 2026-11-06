using System.Linq.Expressions;

namespace WebApplicationStas.Repositories;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, object>> orderBy, params string[] includes);
    Task<int> CountAsync();
    Task<T?> GetByIdAsync(int id, params string[] includes);
    void Add(T entity);
    void Remove(T entity);
    Task<int> SaveChangesAsync();
}
