using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EntityDock.Extensions.Query;

namespace EntityDock.Extensions.Query
{
    public static class QueryExtension
    {
        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, FundamentalQueryModel model)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Filters != null)
            {
                query = query.Filter(model.Filters, model.InclusiveFilters);
            }

            if (model.Sorts != null)
            {
                query = query.OrderBy(model.Sorts);
            }

            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                query = query.Search(model.Search, model.SearchMethod, model.CaseSensitive);
            }

            if (model.Joins != null)
            {
                query = query.IncludeDynamic(model.Joins);
            }

            return query;
        }
    }
}
