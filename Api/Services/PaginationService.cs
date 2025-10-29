using System.Linq.Expressions;
using System.Reflection;

    #region Interface
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
    #endregion
    #region Implementation
///<inheritdoc/>
public class PaginationService : IPaginationService
{
    /// <inheritdoc/>
    public IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortBy, string? sortOrder)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        var rootType = typeof(T);
        var param = Expression.Parameter(rootType, "x");
        Expression propertyAccess = param;
        Type currentType = rootType;

        // support dot-separated nested properties and case-insensitive lookup
        var parts = sortBy.Split('.', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            var prop = currentType.GetProperty(part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop == null)
                throw new ArgumentException($"'{sortBy}' is not a member of type '{rootType.Name}'", nameof(sortBy));

            propertyAccess = Expression.Property(propertyAccess, prop);
            currentType = prop.PropertyType;
        }

        var lambda = Expression.Lambda(propertyAccess, param);

        string methodName = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";

        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(rootType, currentType);

        var result = method.Invoke(null, new object[] { query, lambda });
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
    #endregion




