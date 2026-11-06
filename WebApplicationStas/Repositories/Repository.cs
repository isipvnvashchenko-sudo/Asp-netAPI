using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebApplicationStas.Models;

namespace WebApplicationStas.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ShopdbContext _context;
    private readonly DbSet<T> _set;

    public Repository(ShopdbContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }

    public async Task<List<T>> GetPagedAsync(int page, int pageSize, Expression<Func<T, object>> orderBy, params string[] includes)
    {
        var query = Include(_set.AsNoTracking(), includes);
        return await query.OrderBy(orderBy).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public Task<int> CountAsync() => _set.CountAsync();

    public async Task<T?> GetByIdAsync(int id, params string[] includes)
    {
        var query = Include(_set.AsQueryable(), includes);
        return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
    }

    public void Add(T entity) => _set.Add(entity);

    public void Remove(T entity) => _set.Remove(entity);

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

    private static IQueryable<T> Include(IQueryable<T> query, string[] includes)
    {
        foreach (var include in includes)
            query = query.Include(include);
        return query;
    }
}
