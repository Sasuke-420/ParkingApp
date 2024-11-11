using Lisec.Base.Utilities.ResponseUtilities;
using Lisec.ParkingApp.DTOs;
using Lisec.ParkingApp.Services;
using Lisec.ServiceBase.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Controllers
{
    /// <summary>
    /// RestrictionsController
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class RestrictionsController : BaseController
    {
        private readonly RestrictionsService _service;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="service">Specify RestrictionsService</param>
        public RestrictionsController(RestrictionsService service)
        {
            _service = service;
        }

        /// <summary>
        /// This function gets list of all restrictions from database
        /// </summary>
        /// <param name="query">Specify query</param>
        /// <response code="200">Successfully loaded the restrictions</response>
        /// <response code="204">Content not available</response>
        /// <response code="500">Internal server error</response>
        /// <returns>BaseResponse</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<RestrictionDTO>>> GetAllRestrictionsAsync()
        {
            BaseResponse<IList<RestrictionDTO>> response = await _service.GetAllRestrictionsAsync();
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function updates restrictions with given object
        /// </summary>
        /// <param name="upsertRestrictionRequest">Object of update restriction</param>        
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully updated the restriction</response>
        /// <response code="400">BadRequest</response>
        /// <response code="404">Restriction not found</response>
        /// <response code="409">Conflict error</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RestrictionDTO>> UpsertRestrictionAsync([FromBody] UpsertRestrictionDTO upsertRestrictionRequest)
        {
            BaseResponse<RestrictionDTO> response = await _service.UpsertRestrictionAsync(upsertRestrictionRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function deletes restriction by given id
        /// </summary>
        /// <param name="id">Specify restriction id</param>
        /// <returns>BaseResponse object</returns>        
        /// <response code="200">Successfully deleted the restriction</response>      
        /// <response code="404">Restriction not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRestrictionByIdAsync([FromRoute] int id)
        {
            BaseResponse response = await _service.DeleteRestrictionByIdAsync(id);
            return ReplyBaseResponse(response);
        }
    }
}

