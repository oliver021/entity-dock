using EntityDock.Extensions.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityDock.Queries.ClientSDK
{
    /// <summary>
    /// This model represent a query send by http
    /// </summary>
    public class NestJSModelClient
    {

        /// <summary>
        /// Basic initialziation with rules props
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="inclusive"></param>
        /// <param name="search"></param>
        public NestJSModelClient(IEnumerable<FilterRule> filters, IEnumerable<FilterRule> inclusive, string search)
        {
            if(filters != null ) Filters.AddRange(filters);
            if(inclusive != null) InclusiveFilters.AddRange(inclusive);
            Search = search;
        }

        public NestJSModelClient(IEnumerable<FilterRule> filters, IEnumerable<FilterRule> inclusive, SearchQuery search)
        {
            if (filters != null) Filters.AddRange(filters);
            if (inclusive != null) InclusiveFilters.AddRange(inclusive);
            Search = search.Search;
            SearchMethod = search.Method;
            SearchCaseSensitive= search.CaseSensitive;
        }

        /// <summary>
        /// Paramerless
        /// </summary>
        public NestJSModelClient()
        {
        }

        public List<FilterRule> Filters { get; set; } = new List<FilterRule>();
        public List<FilterRule> InclusiveFilters { get; set; } = new List<FilterRule>();
        public List<KeyValuePair<string, bool>> Sorts { get; set; } = new List<KeyValuePair<string, bool>>();

        public List<string> Joins { get; set; } = new List<string>();

        public List<string> Select { get; set; } = new List<string>();

        public string Search { get; set; } = null;
        public string SearchMethod { get; set; }
        public bool SearchCaseSensitive { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; } 
        public int Page { get; set; } 
        public bool Cache { get; set; } = false;

        /// <summary>
        /// Override this Method
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToQueryString();
        
        /// <summary>
        /// Build a query string paremters
        /// </summary>
        /// <returns></returns>
        public string ToQueryString()
        {
            var collectionOfParams = new List<string>();

            foreach (var item in Filters)
            {
                var q = new object[3] { item.field, item.condition, item.Value};

                collectionOfParams.Add("filter=" + string.Join("||", q.Where(x => x != null)));
            }

            foreach (var item in InclusiveFilters)
            {
                var q = new object[3] { item.field, item.condition, item.Value };

                collectionOfParams.Add("or=" + string.Join("||", q.Where(x => x != null)));
            }

            foreach (var item in Sorts)
            {

                collectionOfParams.Add("sort=" + item.Key + (item.Value ? ",DESC" : string.Empty));
            }

            if (Select.Any())
            {
                collectionOfParams.Add("select=" + string.Join(',', Select));
            }

            if (Joins.Any())
            {
                collectionOfParams.Add("join=" + string.Join(',', Joins));
            }

            if (!string.IsNullOrWhiteSpace(Search))
            {
                collectionOfParams.Add("search=" + Search);
                collectionOfParams.Add("searchMethod=" + SearchMethod ?? nameof(String.Contains));
                if(SearchCaseSensitive)
                    collectionOfParams.Add("searchCase");
            }

            if (Page > 0)
            {
                collectionOfParams.Add("page=" + Page);
            }

            if (Offset > 0)
            {
                collectionOfParams.Add("offset=" + Offset);
            }

            if (Limit > 0)
            {
                collectionOfParams.Add("limit=" + Limit);
            }

            // flush results
            return '?' + string.Join('&', collectionOfParams);
        }
    }
}
