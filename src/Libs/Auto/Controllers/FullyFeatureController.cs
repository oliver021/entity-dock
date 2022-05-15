using System;
using System.Linq;
using System.Threading.Tasks;
using EntityDock.Persistence;
using Microsoft.AspNetCore.Mvc;
using EntityDock.Entities.Base;
using EntityDock.Extensions.Query;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq.Dynamic.Core;

namespace EntityDock.Lib.Auto.Controllers
{
    /// <summary>
    /// Markets crud example with functional Api Systems
    /// </summary>
    public class FullyFeatureController<T, TID> : OperationsController<T, TID>
        where T: AggregateRoot<TID>
    {
        /// <summary>
        /// Require basic data service
        /// </summary>
        /// <param name="service"></param>
        public FullyFeatureController(DataService<T, TID> service) : base(service)
        {
        }

        /// <summary>
        /// Devuelve un listado de todos los registros de datos
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="or"></param>
        /// <param name="join"></param>
        /// <param name="select"></param>
        /// <param name="sort"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="search"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Query(
                  [SwaggerParameter(" Array with rules to filter"), FromQuery] string[] filter,
                  [SwaggerParameter(" Array with rules to allow select other records"), FromQuery] string[] or,
                  [SwaggerParameter(" Array with rules to include relations"), FromQuery] string[] join,
                  [SwaggerParameter(" Array with rules to sort"), FromQuery] string[] select,
                  [SwaggerParameter(" Array with rules to sort") , FromQuery] string[] sort,
                  [SwaggerParameter(" Page that will show")] int page,
                  [SwaggerParameter(" Limit in query or page size if the {page} > 0")] int limit,
                  [SwaggerParameter(" Simple offset parameters to skip records")] int offset,
                  [SwaggerParameter(" Search keywords")] string search,
                  [SwaggerParameter(" Search method")] string searchMethod,
                  [SwaggerParameter(" Case sensitive for Search")] bool caseSensitive,
                  [SwaggerParameter(" Cache enable")] bool cache)
        {
            // full initialize
            var queryModel = new FundamentalQueryModel(filter, or, join,
                                             select: select, sorts: sort, page: page, limit: limit,
                                             offset: offset, search: search, searchMethod: searchMethod,
                                             caseSensitive: caseSensitive, cache: cache);

            // apply filter base on model parameters
            var query = DataService.Repository.Get()
                .ApplyFilter(queryModel);

            // this variable is declared to save the record number
            int count = 0;

            // check
            if (queryModel.Page > 0)
            {
                var pageNumber = queryModel.Page;
                var pageSize = queryModel.Limit;
                pageNumber = pageNumber == 0 ? 1 : pageNumber;
                pageSize = pageSize == 0 ? 10 : pageSize;

                // is necesary do this before apply pagination but with filter applied
                // because the filters determine the total record if has filter in query
                count = await query.CountAsync(); 
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
            else
            {
                if (queryModel.Limit > 0)
                {
                    query = query.Take(queryModel.Limit);
                }

                if (queryModel.Limit > 0)
                {
                    query = query.Skip(queryModel.Offset);
                }
            }

            // container to store results
            object result;

            // define the result base on pagination request
            if (queryModel.Select is not null && queryModel.Select.Length > 0)
            {
                result = await query.SelectFields(queryModel.Select)
                    .ToDynamicListAsync();
            }
            else
            {
                result = await query.ToListAsync();
            }

            // this variable is necesary to store paginated result or simple result
            var finalResult = page > 0 ? new PaginatedResult<object>(true, result, count, page, limit) : result;
            
            // flush result in response
            return Ok(finalResult);
        }

        /// <summary>
        /// Devuelve un unico registro basado en su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [SwaggerResponse(404, "The market data not found by id")]

        public async Task<IActionResult> GetByIdAsync([FromRoute] TID id)
        {
            var result = await DataService.FindAsync(id);
            return result is null ? NotFound() : Ok(value: result);
        }

        /// <summary>
        /// Insert many records by batch income in Body Request
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet("query/get-text")]
        [SwaggerResponse(404, "If the entity not has text fields")]
        public async Task<IActionResult> GetTextAsync()
        {
            var query = await DataService.Repository.Get().SelectText().ToDynamicListAsync();
            return Ok(QueryHelpers.CountAllText(query));
        }

        /// <summary>
        /// Simple download a number of records
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet("query/count")]
        [SwaggerResponse(404, "If the entity not has text fields")]
        public async Task<IActionResult> GetCountAsync(
            [SwaggerParameter(" Array with rules to filter"), FromQuery] string[] filters,
            [SwaggerParameter(" Array with rules to allow select other records"), FromQuery] string[] or,
            [SwaggerParameter(" Search keywords")] string search,
            [SwaggerParameter(" Search method")] string searchMethod,
            [SwaggerParameter(" Case sensitive for Search")] bool caseSensitive)
        {
            var queryModel = new FundamentalQueryModel(filters, or, search, searchMethod, caseSensitive);

            // get count from filter passed
            long count = await DataService.Repository.Get()
                            .ApplyFilter(queryModel)
                            .LongCountAsync();

            // flush count data
            return base.Ok(new {
                count,
                withFilter = filters.Length > 0 || or.Length > 0 || (string.IsNullOrWhiteSpace(search) is false)
            });
        }


        /// <summary>
        /// Delete all record that match with income query
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="or"></param>
        /// <param name="join"></param>
        /// <param name="select"></param>
        /// <param name="sort"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="search"></param>
        /// <param name="searchMethod"></param>
        /// <param name="caseSensitive"></param>
        /// <returns></returns>
        [HttpDelete]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> DeleteQueryResultAsync(
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                  [SwaggerParameter(" Array with rules to filter"), FromQuery] string[] filter,
                  [SwaggerParameter(" Array with rules to allow select other records"), FromQuery] string[] or,
                  [SwaggerParameter(" Array with rules to include relations"), FromQuery] string[] join,
                  [SwaggerParameter(" Array with rules to sort"), FromQuery] string[] select,
                  [SwaggerParameter(" Array with rules to sort"), FromQuery] string[] sort,
                  [SwaggerParameter(" Page that will show")] int page,
                  [SwaggerParameter(" Limit in query or page size if the {page} > 0")] int limit,
                  [SwaggerParameter(" Simple offset parameters to skip records")] int offset,
                  [SwaggerParameter(" Search keywords")] string search,
                  [SwaggerParameter(" Search method")] string searchMethod,
                  [SwaggerParameter(" Case sensitive for Search")] bool caseSensitive)
        {
            return null;
        }
    }

    /// <summary>
    /// Markets crud example with functional Api Systems
    /// </summary>
    public class RepoFullyFeatureController<T, TID> : RepoOperationsController<T, TID>
        where T : AggregateRoot<TID>
    {
        /// <summary>
        /// Require basic data service
        /// </summary>
        /// <param name="service"></param>
        public RepoFullyFeatureController(IRepository<T, TID> service) : base(service)
        {
        }

        /// <summary>
        /// Devuelve un listado de todos los registros de datos
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="or"></param>
        /// <param name="join"></param>
        /// <param name="select"></param>
        /// <param name="sort"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="search"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Query(
                  [SwaggerParameter(" Array with rules to filter"), FromQuery] string[] filter,
                  [SwaggerParameter(" Array with rules to allow select other records"), FromQuery] string[] or,
                  [SwaggerParameter(" Array with rules to include relations"), FromQuery] string[] join,
                  [SwaggerParameter(" Array with rules to sort"), FromQuery] string[] select,
                  [SwaggerParameter(" Array with rules to sort"), FromQuery] string[] sort,
                  [SwaggerParameter(" Page that will show")] int page,
                  [SwaggerParameter(" Limit in query or page size if the {page} > 0")] int limit,
                  [SwaggerParameter(" Simple offset parameters to skip records")] int offset,
                  [SwaggerParameter(" Search keywords")] string search,
                  [SwaggerParameter(" Search method")] string searchMethod,
                  [SwaggerParameter(" Case sensitive for Search")] bool caseSensitive,
                  [SwaggerParameter(" Cache enable")] bool cache)
        {
            // full initialize
            var queryModel = new FundamentalQueryModel(filter, or, join,
                                             select: select, sorts: sort, page: page, limit: limit,
                                             offset: offset, search: search, searchMethod: searchMethod,
                                             caseSensitive: caseSensitive, cache: cache);

            // apply filter base on model parameters
            var query = Repo.Get()
                .ApplyFilter(queryModel);

            // this variable is declared to save the record number
            int count = 0;

            // check
            if (queryModel.Page > 0)
            {
                var pageNumber = queryModel.Page;
                var pageSize = queryModel.Limit;
                pageNumber = pageNumber == 0 ? 1 : pageNumber;
                pageSize = pageSize == 0 ? 10 : pageSize;

                // is necesary do this before apply pagination but with filter applied
                // because the filters determine the total record if has filter in query
                count = await query.CountAsync();
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
            else
            {
                if (queryModel.Limit > 0)
                {
                    query = query.Take(queryModel.Limit);
                }

                if (queryModel.Limit > 0)
                {
                    query = query.Skip(queryModel.Offset);
                }
            }

            // container to store results
            object result;

            // define the result base on pagination request
            if (queryModel.Select is not null && queryModel.Select.Length > 0)
            {
                result = await query.SelectFields(queryModel.Select)
                    .ToDynamicListAsync();
            }
            else
            {
                result = await query.ToListAsync();
            }

            // this variable is necesary to store paginated result or simple result
            var finalResult = page > 0 ? new PaginatedResult<object>(true, result, count, page, limit) : result;

            // flush result in response
            return Ok(finalResult);
        }

        /// <summary>
        /// Devuelve un unico registro basado en su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [SwaggerResponse(404, "The market data not found by id")]

        public async Task<IActionResult> GetByIdAsync([FromRoute] TID id)
        {
            var result = await Repo.GetOne(id);
            return result is null ? NotFound() : Ok(value: result);
        }

        /// <summary>
        /// Insert many records by batch income in Body Request
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet("query/get-text")]
        [SwaggerResponse(404, "If the entity not has text fields")]
        public async Task<IActionResult> GetTextAsync()
        {
            var query = await Repo.Get().SelectText().ToDynamicListAsync();
            return Ok(QueryHelpers.CountAllText(query));
        }

        /// <summary>
        /// Simple download a number of records
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet("query/count")]
        [SwaggerResponse(404, "If the entity not has text fields")]
        public async Task<IActionResult> GetCountAsync(
            [SwaggerParameter(" Array with rules to filter"), FromQuery] string[] filters,
            [SwaggerParameter(" Array with rules to allow select other records"), FromQuery] string[] or,
            [SwaggerParameter(" Search keywords")] string search,
            [SwaggerParameter(" Search method")] string searchMethod,
            [SwaggerParameter(" Case sensitive for Search")] bool caseSensitive)
        {
            var queryModel = new FundamentalQueryModel(filters, or, search, searchMethod, caseSensitive);

            // get count from filter passed
            long count = await Repo.Get()
                            .ApplyFilter(queryModel)
                            .LongCountAsync();

            // flush count data
            return base.Ok(new
            {
                count,
                withFilter = filters.Length > 0 || or.Length > 0 || (string.IsNullOrWhiteSpace(search) is false)
            });
        }


        /// <summary>
        /// Delete all record that match with income query
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="or"></param>
        /// <param name="join"></param>
        /// <param name="select"></param>
        /// <param name="sort"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="search"></param>
        /// <param name="searchMethod"></param>
        /// <param name="caseSensitive"></param>
        /// <returns></returns>
        [HttpDelete]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IActionResult> DeleteQueryResultAsync(
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                  [SwaggerParameter(" Array with rules to filter"), FromQuery] string[] filter,
                  [SwaggerParameter(" Array with rules to allow select other records"), FromQuery] string[] or,
                  [SwaggerParameter(" Array with rules to include relations"), FromQuery] string[] join,
                  [SwaggerParameter(" Array with rules to sort"), FromQuery] string[] select,
                  [SwaggerParameter(" Array with rules to sort"), FromQuery] string[] sort,
                  [SwaggerParameter(" Page that will show")] int page,
                  [SwaggerParameter(" Limit in query or page size if the {page} > 0")] int limit,
                  [SwaggerParameter(" Simple offset parameters to skip records")] int offset,
                  [SwaggerParameter(" Search keywords")] string search,
                  [SwaggerParameter(" Search method")] string searchMethod,
                  [SwaggerParameter(" Case sensitive for Search")] bool caseSensitive)
        {
            return null;
        }
    }
}
