using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace EntityDock.Extensions.Query
{
    /// <summary>
    /// This extensions method use <see cref="System.Linq.Dynamic.Core"/> for dynamic expressions
    /// to make queries
    /// </summary>
    public static class QueryStringExtensions
    {
        /// <summary>
        /// Select several fields of dynamic selection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public static IQueryable SelectFields<T>(this IQueryable<T> query, string[] fields)
        {
            if (fields.Length < 1)
            {
                return query;
            }

            // this statement if necesary because the closure can be 'xxx,xxx,xx' then is necesary to split
            var target = fields.SelectMany(f => f.Contains(',') ? f.Split(',') : new[]{ f });

            // apply selection
            return query.Select("new{ " + string.Join(',', fields) + " }");
        }

        /// <summary>
        /// Filter method with two filter way
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="filter"></param>
        /// <param name="filterInclusive"></param>
        /// <returns></returns>
        public static IQueryable<T> Filter<T>(this IQueryable<T> query, 
            IEnumerable<FilterRule> filter, 
            IEnumerable<FilterRule> filterInclusive = null)
        {
            if (filter is null || filter.Any() is false)
            {
                return query;
            }

            // make filters
            var filterClosure = string.Join("and", filter.Select(f => $"{f.field} {f.condition} {WrapValue<T>(f.field, f.Value)}"));
            var inclusiveClosure = (filterInclusive is null || filterInclusive.Any() is false)
                ? string.Empty :
                " or "+ string.Join("or", filterInclusive.Select(f => $"{f.field} {f.condition} {WrapValue<T>(f.field, f.Value)}"));
            
            // apply where filter
            return query.Where($"({filterClosure}){inclusiveClosure}");
        }

        /// <summary>
        /// Simple helper to wrap with quotes if the prop is 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string WrapValue<T>(string field, object value)
        {
            var prop = typeof(T).GetProperty(field);

            if (prop.PropertyType.Name.Equals(nameof(String)))
            {
                return $"\"{value}\"";
            }
            else
            {
                return value.ToString(); ;
            }
        }

        /// <summary>
        /// Order dynamic method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="sorts"></param>
        /// <returns></returns>
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, IEnumerable<KeyValuePair<string, bool>> sorts)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (!sorts.Any())
                return query; // avoid use this query if sorts rule is empty
            // make sort
            return query.OrderBy(string.Join(',', sorts.Select(s => s.Key +(s.Value ? " desc" : ""))));
        }

        /// <summary>
        /// GetText return a query that select alls text about an entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public static IQueryable SelectText<T>(this IQueryable<T> query)
        {
            // this statement if necesary because the closure can be 'xxx,xxx,xx' then is necesary to split
            var fields = typeof(T).GetPropertiesMapped()
                .Where(x => x.PropertyType.IsEquivalentTo(typeof(string)))
                .Select(x => x.Name);

            // apply selection
            return query.Select("new{ " + string.Join(',', fields) + " }");
        }
    }
}
