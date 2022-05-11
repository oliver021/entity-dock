using EntityDock.Extensions.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityDock.Queries.ClientSDK
{
    public class QueryNestJSBuilder
    {
        public NestJSModelClient ModelClient { get; } = new NestJSModelClient();

        public QueryNestJSBuilder AddFilter(FilterRule rule)
        {
            ModelClient.Filters.Add(rule);
            return this;
        }

        public QueryNestJSBuilder Include(FilterRule rule)
        {
            ModelClient.InclusiveFilters.Add(rule);
            return this;
        }

        public QueryNestJSBuilder AddSort(KeyValuePair<string,bool> sort)
        {
            ModelClient.Sorts.Add(sort);
            return this;
        }

        public QueryNestJSBuilder AddSelect(string select)
        {
            ModelClient.Select.Add(select);
            return this;
        }

        public QueryNestJSBuilder AddRel(string rel)
        {
            ModelClient.Joins.Add(rel);
            return this;
        }

        public QueryNestJSBuilder Page(int page, int len)
        {
            ModelClient.Page = page;
            ModelClient.Limit = len;
            return this;
        }

        public QueryNestJSBuilder Search(string term)
        {
            ModelClient.Search = term;
            return this;
        }
    }
}
