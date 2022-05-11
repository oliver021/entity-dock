using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityDock
{
    /// <summary>
    /// PaginatedResult is container for record lis of the entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedResult<T> : Result
    {
        /// <summary>
        /// Simple construct
        /// </summary>
        /// <param name="data"></param>
        public PaginatedResult(T data)
        {
            Data = data;
        }

        /// <summary>
        /// 
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Construct for paginated record cases
        /// </summary>
        /// <param name="succeeded"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        public PaginatedResult(bool succeeded, T data = default, int count = 0, int page = 1, int pageSize = 10)
        {
            IsPaginated = true;
            Data = data;
            CurrentPage = page;
            Succeeded = succeeded;
            PageSize = pageSize;
            TotalPages = (int) Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
        }

        /// <summary>
        /// Construct for non result cases
        /// </summary>
        /// <param name="succeeded"></param>
        /// <param name="data"></param>
        /// <param name="count"></param>
        public PaginatedResult(bool succeeded, T data = default, int count = 0)
        {
            Data = data;
            Succeeded = succeeded;
            TotalCount = count;
        }



        /// <summary>
        /// Parameterless constructor, usefull for example to deserialize a json with this Type
        /// </summary>
        public PaginatedResult()
        {
        }

        /// <summary>
        /// For Failure cases
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PaginatedResult<T> Failure(int page, int pageSize)
        {
            return new PaginatedResult<T>(false, default, 0, page, pageSize);
        }

        /// <summary>
        /// For success cases pass the record to paginate
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PaginatedResult<T> Success(T data, int count, int page, int pageSize)
        {
            return new PaginatedResult<T>(true, data, count, page, pageSize);
        }

        /// <summary>
        /// Initialize non paginate record container
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static PaginatedResult<T> NonPaginate(T data, int count)
        {
            return new PaginatedResult<T>(true, data, count);
        }

        public bool IsPaginated { get; set; } = false;

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
