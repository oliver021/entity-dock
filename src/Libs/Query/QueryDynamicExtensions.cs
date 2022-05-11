using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityDock.Extensions.Query
{
    /// <summary>
    /// This class define extensions for <see cref="IQueryable{T}"/> to make dynamic queries
    /// </summary>
    public static class QueryDynamicExtensions
    {
        /*search methods*/
        public const string SearchMethodContains = "Contains";
        public const string SearchMethodStart = nameof(String.StartsWith);
        public const string SearchMethodEnd = nameof(String.EndsWith);

        /* operation constants */
        public const string EqualOperation = "equal";
        public const string DiffOperation = "diff";
        public const string GreaterOperation = "Greater";
        public const string GreaterEqualOperation = "Greater-equal";
        public const string LessOperation = "less";
        public const string LessEqualOperation = "less-equal";

        /// <summary>
        /// Order by dynamic expresions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="orderByMember"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static IQueryable<T> IncludeDynamic<T>(this IQueryable<T> query, string[] relationsShips)
        {
            if (relationsShips is null || relationsShips.Length < 1)
            {
                return query;
            }

            IQueryable<T> result = query;

            // iterate with all relationships
            foreach (var item in relationsShips)
            {
                result = result.IncludeDynamic(item);
            }

            return result;
        }

        /// <summary>
        /// Set dynamic relationship loads
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="relationShip"></param>
        /// <returns></returns>
        private static IQueryable<T> IncludeDynamic<T>(this IQueryable<T> query, string relationShip)
        {
            Type entity = typeof(T);
            var queryElementTypeParam = Expression.Parameter(entity);
            var memberAccess = Expression.PropertyOrField(queryElementTypeParam, relationShip);
            var keySelector = Expression.Lambda(memberAccess, queryElementTypeParam);

            var orderBy = Expression.Call(
                typeof(Queryable),
                 "Include",
                new Type[] { entity, memberAccess.Type },
                query.Expression,
                Expression.Quote(keySelector));

            return query.Provider.CreateQuery<T>(orderBy);
        }

        /// <summary>
        /// Order by dynamic expresions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="orderByMember"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string orderByMember, bool desc)
        {
            if (string.IsNullOrEmpty(orderByMember))
            {
                return query;
            }

            Type entity = typeof(T);
            var queryElementTypeParam = Expression.Parameter(entity);
            var memberAccess = Expression.PropertyOrField(queryElementTypeParam, orderByMember);
            var keySelector = Expression.Lambda(memberAccess, queryElementTypeParam);

            var orderBy = Expression.Call(
                typeof(Queryable),
                desc ? "OrderByDescending" : "OrderBy",
                new Type[] { entity, memberAccess.Type },
                query.Expression,
                Expression.Quote(keySelector));

            return query.Provider.CreateQuery<T>(orderBy);
        }

        /// <summary>
        /// Dynamically where helper
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="entity"></param>
        /// <param name="keyField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereDynamic<T>(this IQueryable<T> query,
            string keyField,
            object value,
            string method = EqualOperation)
        {
            if (string.IsNullOrEmpty(keyField))
            {
                return query;
            }

            // header preset parameters
            Type entity = typeof(T);
            ParameterExpression parameterExpression = Expression.Parameter(entity, "record");
            ConstantExpression constantExpression = Expression.Constant(value);

            // find the expressions
            BinaryExpression operationExp = CreateEvaluationExpresion(keyField, method, parameterExpression, constantExpression);

            // emulate lambda expresion closure
            var expression = Expression.Lambda(operationExp, parameterExpression);

            // make a call
            MethodCallExpression methodCallExpression = Expression.Call(
            typeof(Queryable),
            "Where",
            new[] { entity },
            query.Expression, Expression.Quote(expression));
            return query.Provider.CreateQuery<T>(methodCallExpression);
        }

        /// <summary>
        /// Helper to create evaluation expressions
        /// </summary>
        /// <param name="keyField"></param>
        /// <param name="method"></param>
        /// <param name="parameterExpression"></param>
        /// <param name="constantExpression"></param>
        /// <returns></returns>
        private static BinaryExpression CreateEvaluationExpresion(string keyField, string method, ParameterExpression parameterExpression, ConstantExpression constantExpression)
        {
            // define a operation by argument passed
            return method switch
            {
                EqualOperation => Expression.Equal(left: Expression.Property(parameterExpression, keyField), right: constantExpression),
                DiffOperation => Expression.NotEqual(left: Expression.Property(parameterExpression, keyField), right: constantExpression),
                GreaterOperation => Expression.GreaterThan(left: Expression.Property(parameterExpression, keyField), right: constantExpression),
                GreaterEqualOperation => Expression.GreaterThanOrEqual(left: Expression.Property(parameterExpression, keyField), right: constantExpression),
                LessOperation => Expression.LessThan(left: Expression.Property(parameterExpression, keyField), right: constantExpression),
                LessEqualOperation => Expression.LessThanOrEqual(left: Expression.Property(parameterExpression, keyField), right: constantExpression),
                _ => throw new NotSupportedException()
            };
        }

        /// <summary>
        /// Make a full search text by all string fields or selected fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="searchText"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static IQueryable<T> Search<T>(this IQueryable<T> query,
            string searchText,
            string searchType = SearchMethodContains,
            bool searchCaseSensitive = false,
            string[] fields = null)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return query;
            }

            Type type = typeof(T);
            var methodInfo = typeof(string).GetMethod(searchType, new Type[] { typeof(string) });
            var parameter = Expression.Parameter(type, "sa");
            var constant = Expression.Constant(searchText);

            // make implementation of the expression
            var expressions = type.GetProperties()
                .Where(prop => prop.PropertyType == typeof(string) && (fields is null || fields.Contains(prop.Name)))
                .Select(prop =>
                {
                    var member = Expression.Property(parameter, prop.Name);
                    Expression body = Expression.Call(member, methodInfo, constant);
                    return Expression.Lambda(body, parameter);
                }
                ).ToList();

            // lambda declaration
            LambdaExpression keySelector;

            // check case
            if (expressions.Count == 1)
            {
                keySelector = expressions[0];
            }
            else
            {
                var orExpression = expressions.Skip(2).Aggregate(
                    Expression.OrElse(expressions[0].Body, Expression.Invoke(expressions[1], expressions[0].Parameters[0])),
                    (x, y) => Expression.OrElse(x, Expression.Invoke(y, expressions[0].Parameters[0])));
                keySelector = Expression.Lambda(orExpression, expressions[0].Parameters);
            }

            // make call where
            MethodCallExpression whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { type },
                query.Expression,
                Expression.Quote(keySelector));

            return query.Provider.CreateQuery<T>(whereCallExpression);
        }
    }
}
