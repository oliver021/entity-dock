using EntityDock.Extensions.Query;
using EntityDock.Queries.ClientSDK;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketCrud.UI.Client.Helpers
{
    /// <summary>
    /// Helpers
    /// </summary>
    public static class HelpersExtensions
    {
        /// <summary>
        /// Build an model to create a query from table state and filters passed
        /// </summary>
        /// <param name="table"></param>
        /// <param name="filters"></param>
        /// <param name="inclusive"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static NestJSModelClient BuildQuery(this TableState table, IEnumerable<FilterRule> filters, IEnumerable<FilterRule> inclusive, SearchQuery search = default)
        {
            if (table is null)
            {
                throw new ArgumentNullException(nameof(table));
            }
            
            int passedPage = table.Page + 1;

            var model = new NestJSModelClient(filters, inclusive, search)
            {
                Page = passedPage == 0 ? 1 : passedPage,
                Limit = table.PageSize
            };
            Console.WriteLine("page size:{0} - {1}",table.PageSize, table.Page);
            if (!string.IsNullOrEmpty(table.SortLabel))
                model.Sorts.Add(KeyValuePair.Create(table.SortLabel, table.SortDirection == SortDirection.Descending));

            return model;
        }
    }
}
