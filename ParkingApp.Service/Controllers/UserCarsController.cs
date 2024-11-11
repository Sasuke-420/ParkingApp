using Lisec.Base.Utilities.ResponseUtilities;
using Lisec.ParkingApp.DTOs;
using Lisec.ParkingApp.Services;
using Lisec.ServiceBase.Controllers;
using Lisec.ServiceBase.QueryFilter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Controllers
{
    /// <summary>
    /// UserCarsController
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserCarsController : BaseController
    {
        private readonly UserCarsService _service;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="service">Specify UserCarsService</param>
        public UserCarsController(UserCarsService service)
        {
            _service = service;
        }

        /// <summary>
        /// This function gets list of all user cars from database
        /// </summary>
        /// <param name="query">Specify query</param>
        /// <response code="200">Successfully loaded the user cars</response>
        /// <response code="204">Content not available</response>
        /// <response code="500">Internal server error</response>
        /// <returns>BaseResponse</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<UserCarDTO>>> GetAllUserCarsAsync([FromQuery] Query query = null)
        {
            BaseResponse<IList<UserCarDTO>> response = await _service.GetAllUserCarsAsync(query);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function get user car by id
        /// </summary>
        /// <param name="id">Specify user car id</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully loaded the user car</response>
        /// <response code="404">Card not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserCarDTO>> GetUserCarsByIdAsync([FromRoute] int id)
        {
            BaseResponse<UserCarDTO> response = await _service.GetUserCarByIdAsync(id);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function adds user car record in Database
        /// </summary>
        /// <param name="createUserCarRequest">Object of create and update user car</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="201">Successfully added the user car record</response>
        /// <response code="400">BadRequest</response>
        /// <response code="412">USer not exist</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserCarDTO>> AddUserCarAsync([FromBody] CreateUserCarDTO createUserCarRequest)
        {
            BaseResponse<UserCarDTO> response = await _service.AddUserCarAsync(createUserCarRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function updates user car with given object
        /// </summary>
        /// <param name="id">Specify user car id</param>
        /// <param name="updateUserCarRequest">Object of update user car</param>        
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully updated the user car</response>
        /// <response code="400">BadRequest</response>
        /// <response code="404">User car not found</response>
        /// /// <response code="412">USer not exist</response>
        /// <response code="409">Conflict error</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserCarDTO>> UpdateUserCarasync([FromRoute] int id, [FromBody] CreateUserCarDTO updateUserCarRequest)
        {
            BaseResponse<UserCarDTO> response = await _service.UpdateUserCarAsync(id, updateUserCarRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function deletes user car by given id
        /// </summary>
        /// <param name="id">Specify user car id</param>
        /// <returns>BaseResponse object</returns>        
        /// <response code="200">Successfully deleted the user car</response>      
        /// <response code="404">User car not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserCarByIdAsync([FromRoute] int id)
        {
            BaseResponse response = await _service.DeleteUserCarByIdAsync(id);
            return ReplyBaseResponse(response);
        }
    }
}
