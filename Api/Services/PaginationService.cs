using System.Linq;
using System.Linq.Expressions;

public interface IPaginationService
{
    IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortBy, string? sortOrder);
    IQueryable<T> ApplyPagination<T>(IQueryable<T> query, int? page, int? pageSize);
}


public class PaginationService : IPaginationService
{
    public IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortBy, string? sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
            return query;

        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(param, sortBy);
        var lambda = Expression.Lambda(property, param);

        string methodName = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";
        var result = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type)
            .Invoke(null, new object[] { query, lambda });

        return (IQueryable<T>)result!;
    }

    public IQueryable<T> ApplyPagination<T>(IQueryable<T> query, int? page, int? pageSize)
    {
        int currentPage = page ?? 1;
        int currentPageSize = pageSize ?? 20;
        return query.Skip((currentPage - 1) * currentPageSize).Take(currentPageSize);
    }
}