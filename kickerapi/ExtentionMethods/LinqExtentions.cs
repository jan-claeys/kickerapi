using System.Linq.Expressions;

namespace System.Linq
{
    public static class LinqExtentions
    {
        //If value is null, then the expression is not applied
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, Expression<Func<T, bool>> expresssion, object? value)
        {
            return value is null ? query : query.Where(expresssion);
        }

        //If value is false, then the expression is not applied
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, Expression<Func<T, bool>> expresssion, bool value)
        {
            return value ? query.Where(expresssion) : query;
        }
    }
}
