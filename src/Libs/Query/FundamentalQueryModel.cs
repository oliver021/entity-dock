using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace EntityDock.Extensions.Query
{
    /// <summary>
    /// The nextjs model to proces query parameters to filter, paginate and more
    /// </summary>
    public class FundamentalQueryModel : IEquatable<FundamentalQueryModel>
    {
        /// <summary>
        /// Helper to fetch data model from http query parameters <see cref="IQueryCollection"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static FundamentalQueryModel FromQueryRequest(HttpRequest request)
        {
            var model = new FundamentalQueryModel();
            foreach (KeyValuePair<string, StringValues> item in request.Query)
            {
                switch (item.Key)
                {
                    case "join":
                        model.Joins = item.Value.ToArray();
                        break;

                    case "select":
                        model.Select = item.Value.ToArray();
                        break;

                    case "search":
                        model.Search = item.Value.ToString();
                        break;

                    case "page":
                        model.Page = int.Parse(item.Value.ToString());
                        break;

                    case "limit":
                        model.Limit = int.Parse(item.Value.ToString());
                        break;

                    case "offset":
                        model.Offset = int.Parse(item.Value.ToString());
                        break;

                    case "cache":
                        model.Cache = int.Parse(item.Value.ToString()) == 1;
                        break;

                    case "filter":
                        model.Filters = item.Value.Select(d => MapFilterRule(d));
                        break;

                    case "or":
                        model.InclusiveFilters = item.Value.Select(closure => MapFilterRule(closure));
                        break;

                    case "sort":
                        SetSortsRules(model, item.Value);
                        break;

                    default:
                        continue;
                }
            }

            return model;
        }

        /// <summary>
        /// Simple helper to set rules for sorting
        /// </summary>
        /// <param name="model"></param>
        /// <param name="current"></param>
        private static void SetSortsRules(FundamentalQueryModel model, StringValues current)
        {
            model.Sorts = current.Select(closure =>
            {
                var p = closure.Split(',');
                return KeyValuePair.Create(p[0], p.Length > 1 && p[1] == "DESC");
            });
        }

        /// <summary>
        /// Help to parse a query value with format {field||$condition||arg}
        /// </summary>
        /// <param name="closure"></param>
        /// <returns></returns>
        internal static FilterRule MapFilterRule(string closure)
        {
            var segs = closure.Split("||");

            return new FilterRule(segs[0], GetOperator(segs[1]), segs.Length > 2 ? segs[2] : null);
        }


        /// <summary>
        /// Resolve some operators
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetOperator(string name)
        => name switch
        {
            "$eq" => "==",
            "$ne" => "!=",
            "$gt" => ">",
            "$gte" => ">=",
            "$lte" => "<=",
            "$le" => "<",
            "$isnull" => "is",
            "$notnull " => "is not",
            _ => "==",
        };

        /// <summary>
        /// Paraemterless to custom initialziation
        /// </summary>
        public FundamentalQueryModel()
        {

        }

        /// <summary>
        /// Full initialization of the model
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="or"></param>
        /// <param name="join"></param>
        /// <param name="select"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="search"></param>
        /// <param name="cache"></param>
        public FundamentalQueryModel(StringValues filters,
                           StringValues or,
                           StringValues join,
                           StringValues select,
                           StringValues sorts,
                           string page,
                           string limit,
                           string offset,
                           string search,
                           string searchMethod,
                           bool caseSensitive,
                           string cache)
        {
            
            Page = int.Parse(page);
            Limit = int.Parse(limit);
            Offset = int.Parse(offset);
            Cache = int.Parse(cache) == 1;

            SearchMethod = searchMethod;
            CaseSensitive = caseSensitive;

            Search = search;
            Filters = filters.Select(d => MapFilterRule(d));
            InclusiveFilters = or.Select(d => MapFilterRule(d));
            Joins = join.ToArray();
            Select = select.ToArray();
            SetSortsRules(this, sorts);
        }

        /// <summary>
        /// Full initialization of the model
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="or"></param>
        /// <param name="join"></param>
        /// <param name="select"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="search"></param>
        /// <param name="cache"></param>
        public FundamentalQueryModel(StringValues filters,
                          StringValues or,
                          StringValues join,
                          StringValues select,
                          StringValues sorts,
                          int page,
                          int limit,
                          int offset,
                          string search,
                          string searchMethod,
                          bool caseSensitive,
                          bool cache)
        {

            Page = page;
            Limit = limit;
            Offset = offset;
            Cache = cache;
            Search = search;

            SearchMethod = searchMethod;
            CaseSensitive = caseSensitive;

            Filters = filters.Select(d => MapFilterRule(d));
            InclusiveFilters = or.Select(d => MapFilterRule(d));
            Joins = join.ToArray();
            Select = select.ToArray();
            
            SetSortsRules(this, sorts);
        }

        /// <summary>
        /// Full initialization of the model
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="or"></param>
        /// <param name="join"></param>
        /// <param name="select"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="search"></param>
        /// <param name="cache"></param>
        public FundamentalQueryModel(string[] filters,
                          string[] or,
                          string[] join,
                          string[] select,
                          string[] sorts,
                          int page,
                          int limit,
                          int offset,
                          string search,
                          string searchMethod,
                          bool caseSensitive,
                          bool cache)
        {

            Page = page;
            Limit = limit;
            Offset = offset;
            Cache = cache;
            Search = search;
            SearchMethod = searchMethod;
            CaseSensitive = caseSensitive;
            Filters = filters.Select(d => MapFilterRule(d));
            InclusiveFilters = or.Select(d => MapFilterRule(d));
            Joins = join;
            Select = select;

            SetSortsRules(this, sorts);
        }

        /// <summary>
        /// Just filter features
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="inclusiveFilters"></param>
        /// <param name="search"></param>
        /// <param name="searchMethod"></param>
        /// <param name="caseSensitive"></param>
        public FundamentalQueryModel(string[] filters, string[] or, string search, string searchMethod, bool caseSensitive)
        {
            Filters = filters.Select(d => MapFilterRule(d));
            InclusiveFilters = or.Select(d => MapFilterRule(d));
            Search = search;
            SearchMethod = searchMethod;
            CaseSensitive = caseSensitive;
        }

        public IEnumerable<FilterRule> Filters { get; set; }
        public IEnumerable<FilterRule> InclusiveFilters { get; set; }
        public IEnumerable<KeyValuePair<string, bool>> Sorts { get; set; }
        public string[] Joins { get; set; }

        public string[] Select { get; set; }
        public string Search { get; set; } = null;
        public string SearchMethod { get; set; } = nameof(string.Contains);
        public bool CaseSensitive { get; set; } = false;

        public int Limit { get; set; } = 0;
        public int Offset { get; set; } = 0;
        public int Page { get; set; } = 0;
        public bool Cache { get; set; } = false;

        /// <summary>
        /// Override to compare with other instances
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as FundamentalQueryModel);
        }

        /// <summary>
        /// Override to compare with other instances
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(FundamentalQueryModel other)
        {
            return other != null &&
                   EqualityComparer<IEnumerable<FilterRule>>.Default.Equals(Filters, other.Filters) &&
                   EqualityComparer<IEnumerable<FilterRule>>.Default.Equals(InclusiveFilters, other.InclusiveFilters) &&
                   EqualityComparer<IEnumerable<KeyValuePair<string, bool>>>.Default.Equals(Sorts, other.Sorts) &&
                   EqualityComparer<string[]>.Default.Equals(Joins, other.Joins) &&
                   EqualityComparer<string[]>.Default.Equals(Select, other.Select) &&
                   Search == other.Search &&
                   Limit == other.Limit &&
                   Offset == other.Offset &&
                   Page == other.Page &&
                   Cache == other.Cache;
        }

        /// <summary>
        /// Override hash code method to process all values
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Filters);
            hash.Add(InclusiveFilters);
            hash.Add(Sorts);
            hash.Add(Joins);
            hash.Add(Select);
            hash.Add(Search);
            hash.Add(Limit);
            hash.Add(Offset);
            hash.Add(Page);
            hash.Add(Cache);
            return hash.ToHashCode();
        }

        public static bool operator ==(FundamentalQueryModel left, FundamentalQueryModel right)
        {
            return EqualityComparer<FundamentalQueryModel>.Default.Equals(left, right);
        }

        public static bool operator !=(FundamentalQueryModel left, FundamentalQueryModel right)
        {
            return !(left == right);
        }
    }
}
