using EntityDock.Entities.Base;
using EntityDock.Persistence;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EntityDock.Lib.Auto.Controllers
{
    /// <summary>
    /// Markets crud example with functional Api Systems
    /// </summary>
    [ApiController]
    public abstract class OperationsController<T, TID> : ControllerBase
        where T : AggregateRoot<TID>
    {
        /// <summary>
        /// Require basic data service
        /// </summary>
        /// <param name="service"></param>
        public OperationsController(DataService<T, TID> service)
        {
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            DataService = service;
        }

        /// <summary>
        /// Reference of the active service for this entity
        /// </summary>
        public DataService<T, TID> DataService { get; set; }

        /// <summary>
        /// Actualiza un registro con datos que entran en el cuerpo de la peticion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerResponse(204, "The market is updated")]
        [SwaggerResponse(400, "The market data is invalid")]
        public async Task<NoContentResult> UpdateAsync([FromQuery] TID id, [FromBody] T data)
        {
            try
            {
                await DataService.UpdateAsync(id, data);
            }
            catch (Exception)
            {
                // ignore for now
            }
            return NoContent();
        }

        /// <summary>
        /// Elimina un registro basado en su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<NoContentResult> DeleteAsync(
            [SwaggerParameter("Id to delete a market record"), FromRoute] TID id)
        {
            try
            {
                await DataService.RemoveAsync(id);
            }
            catch (Exception)
            {
                // ignore for now
            }
            return NoContent();
        }

        /// <summary>
        /// Agrega un registro con datos que entran en el cuerpo de la peticion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(400, "The market data is invalid")]
        public async Task<IActionResult> AddAsync([FromBody] T data)
        {
            try
            {
                var result = await DataService.InsertRecord(data);
                return StatusCode(201, new
                {
                    result.Result,
                });
            }
            catch (Exception)
            {
                // ignore for now
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Insert many records by batch income in Body Request
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("batch")]
        [SwaggerResponse(204, "If the code is 204 then the operation is successful")]
        public async Task<IActionResult> AddBatchAsync([FromBody] IEnumerable<T> data)
        {
            try
            {
                await DataService.InsertBatch(data);
                return NoContent();
            }
            catch (Exception)
            {
                // ignore for now
                return StatusCode(400);
            }
        }
    }
}
