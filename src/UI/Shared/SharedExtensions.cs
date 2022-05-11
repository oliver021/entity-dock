using EntityDock.Queries.ClientSDK;
using MarketCrud.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MarketCrud.UI.Shared
{
    public static class SharedExtensions
    {
        /// <summary>
        /// Fetch records from uri endpoint and query base on model <see cref="NestJSModelClient"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="uri"></param>
        /// <param name="modelClient"></param>
        /// <returns></returns>
        public static Task<PaginatedResult<List<T>>> ExecuteQuery<T>(this HttpClient client, string uri, NestJSModelClient modelClient)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            
            Console.WriteLine("debug query tring:{0}",modelClient.ToQueryString());

            return client.GetFromJsonAsync<PaginatedResult<List<T>>>(uri + modelClient.ToQueryString());
        }
    }
}
