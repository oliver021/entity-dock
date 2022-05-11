using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EntityDock.Entities.Base;
using Microsoft.EntityFrameworkCore;

namespace EntityDock
{
    /// <summary>
    /// Extensiones de consulta generalmente aplicadas a <see cref="IQueryable{T}"/>
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Build a list from db and mapped all results
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static async Task<List<TDestination>> MapListAsync<T, TDestination>(this IQueryable<T> source, IMapper mapper)
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            var data = await source.ToArrayAsync();

            return data.Select(d => mapper.Map<TDestination>(d)).ToList();
        }

        /// <summary>
        /// Build a list from db and mapped all results
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static async Task<TDestination[]> MapArrayAsync<T, TDestination>(this IQueryable<T> source, IMapper mapper)
        {
            if (mapper is null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            var data = await source.ToArrayAsync();

            return data.Select(d => mapper.Map<TDestination>(d)).ToArray();
        }

        /// <summary>
        /// Simple mapper projection for <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static IEnumerable<TDestination> UseMapper<T, TDestination>(this IEnumerable<T> source, IMapper mapper)
        {
            return source.Select(d => mapper.Map<TDestination>(d));
        }

        /// <summary>
        /// Get pagination result about an entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PaginatedResult<List<T>>> ToPaginatedListAsync<T>(this IQueryable<T> source,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        where T : class
        {
            if (source == null) throw new InvalidOperationException();
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;
            int count = await source.CountAsync();
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return PaginatedResult<List<T>>.Success(items, count, pageNumber, pageSize);
        }

        /// <summary>
        /// Get pagination result about an entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PaginatedResult<List<T>>> ToNonPaginatedRecordAsync<T>(this IQueryable<T> source,
            CancellationToken cancellationToken = default)
        where T : class
        {
            if (source == null) throw new InvalidOperationException();
            int count = await source.CountAsync();
            List<T> items = await source.ToListAsync(cancellationToken);
            return PaginatedResult<List<T>>.NonPaginate(items, count);
        }
    }
}
