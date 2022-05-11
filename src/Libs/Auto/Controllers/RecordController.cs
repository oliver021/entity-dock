using EntityDock.Entities.Base;
using EntityDock.Extensions.Query;
using EntityDock.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace EntityDock.Lib.Auto.Controllers
{
    /// <summary>
    /// Simple readed controller
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TID"></typeparam>
    public class RecordController<T, TID> : OperationsController<T, TID> where T : AggregateRoot<TID>
    {
        public RecordController(DataService<T, TID> service) : base(service)
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
        public async Task<IActionResult> QueryMarkets(
              [SwaggerParameter(" Page that will show")] int page,
              [SwaggerParameter(" Limit in query or page size if the {page} > 0")] int limit,
              [SwaggerParameter(" Cache enable")] bool cache)
        {
            // full initialize
            var queryModel = new FundamentalQueryModel();

            // apply filter base on model parameters
            var query = DataService.Repository.Get()
                .ApplyFilter(queryModel);


            // flush result in response
            return Ok(await query.ToListAsync());
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
    }
}
