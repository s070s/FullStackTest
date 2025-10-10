using System.Linq.Expressions;

/// <summary>
/// Service interface for applying sorting and pagination to IQueryable queries.
/// </summary>
public interface IPaginationService
{
    /// <summary>
    /// Applies sorting to the query based on the specified property and order.
    /// </summary>
    /// <typeparam name="T">Type of the query elements.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="sortBy">The property name to sort by.</param>
    /// <param name="sortOrder">The sort order ("asc" or "desc").</param>
    /// <returns>The sorted query.</returns>
    IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortBy, string? sortOrder);

    /// <summary>
    /// Applies pagination to the query based on the specified page and page size.
    /// </summary>
    /// <typeparam name="T">Type of the query elements.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>The paginated query.</returns>
    IQueryable<T> ApplyPagination<T>(IQueryable<T> query, int? page, int? pageSize);
}

public class PaginationService : IPaginationService
{
    /// <inheritdoc/>
    public IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortBy, string? sortOrder)
    {
        if (string.IsNullOrEmpty(sortBy))
            return query;

        // Build a lambda expression for the property to sort by (e.g., x => x.Property)
        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(param, sortBy);
        var lambda = Expression.Lambda(property, param);

        // Determine the sorting method based on sortOrder
        string methodName = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";

        // Use reflection to call the appropriate OrderBy/OrderByDescending method
        var result = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type)
            .Invoke(null, new object[] { query, lambda });

        return (IQueryable<T>)result!;
    }

    /// <inheritdoc/>
    public IQueryable<T> ApplyPagination<T>(IQueryable<T> query, int? page, int? pageSize)
    {
        int currentPage = page ?? 1;
        int currentPageSize = pageSize ?? 20;
        // Skip items from previous pages and take the current page's items
        return query.Skip((currentPage - 1) * currentPageSize).Take(currentPageSize);
    }
}