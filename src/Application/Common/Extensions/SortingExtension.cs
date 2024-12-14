using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Extensions;
public static class SortingExtension
{
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string columnName, bool isDescending = false)
    {
        if (string.IsNullOrEmpty(columnName))
        {
            return source;
        }

        var propertyInfo = typeof(T).GetProperty(columnName);
        if (propertyInfo == null)
        {
            throw new ArgumentException($"Column '{columnName}' does not exist in type '{typeof(T).Name}'.");
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        MemberExpression property = Expression.Property(parameter, columnName);
        LambdaExpression lambda = Expression.Lambda(property, parameter);

        string methodName = isDescending ? "OrderByDescending" : "OrderBy";

        Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
            new Type[] { typeof(T), property.Type },
            source.Expression, Expression.Quote(lambda));

        return source.Provider.CreateQuery<T>(methodCallExpression);
    }
}
