using System.Linq.Expressions;

namespace System.Linq
{
    public static class LinqExtentions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, Expression<Func<T, bool>> expresssion, object? value)
        {
            return value is null ? query : query.Where(expresssion);
        }

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, Expression<Func<T, bool>> expresssion, bool value)
        {
            return value ? query.Where(expresssion) : query;
        }
    }
}
