using Lisec.Base.Utilities.ResponseUtilities;
using Lisec.ParkingApp.DTOs;
using Lisec.ParkingApp.Services;
using Lisec.ServiceBase.Authentication;
using Lisec.ServiceBase.Controllers;
using Lisec.ServiceBase.QueryFilter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Controllers
{
    /// <summary>
    /// PaidParkingsController
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class PaidParkingsController : BaseController
    {
        private readonly PaidParkingsService _service;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="service">Specify PaidParkingsService</param>
        public PaidParkingsController(PaidParkingsService service)
        {
            _service = service;
        }

        /// <summary>
        /// This function gets list of all paid parkings from database
        /// </summary>
        /// <param name="query">Specify query</param>
        /// <response code="200">Successfully loaded the paid parkings</response>
        /// <response code="204">Content not available</response>
        /// <response code="500">Internal server error</response>
        /// <returns>BaseResponse</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<PaidParkingDTO>>> GetAllPaidParkingsAsync([FromQuery] Query query = null)
        {
            BaseResponse<IList<PaidParkingDTO>> response = await _service.GetAllPaidParkingsAsync(query);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function get paid parking by id
        /// </summary>
        /// <param name="id">Specify paid parking id</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully loaded the paid parking</response>
        /// <response code="404">PaidParking not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaidParkingDTO>> GetPaidParkingsByIdAsync([FromRoute] int id)
        {
            BaseResponse<PaidParkingDTO> response = await _service.GetPaidParkingByIdAsync(id);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function adds paid parking record in Database
        /// </summary>
        /// <param name="createPaidParkingRequest">Object of create and update paid parking</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="201">Successfully added the paid parking record</response>
        /// <response code="400">BadRequest</response>
        /// <response code="412">Specified user id doesn't exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaidParkingDTO>> AddPaidParkingAsync([FromBody] CreatePaidParkingDTO createPaidParkingRequest)
        {
            BaseResponse<PaidParkingDTO> response = await _service.AddPaidParkingAsync(createPaidParkingRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function updates paid parkings with given object
        /// </summary>
        /// <param name="id">Specify paid parking id</param>
        /// <param name="updatePaidParkingRequest">Object of update paid parking</param>        
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully updated the paid parking</response>
        /// <response code="400">BadRequest</response>
        /// <response code="404">PaidParking not found</response>
        /// <response code="412">Specified user id doesn't exists</response>
        /// <response code="409">Conflict error</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaidParkingDTO>> UpdatePaidParkingAsync([FromRoute] int id, [FromBody] CreatePaidParkingDTO updatePaidParkingRequest)
        {
            BaseResponse<PaidParkingDTO> response = await _service.UpdatePaidParkingAsync(id, updatePaidParkingRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function minimizes transactions
        /// </summary>
        /// <param name="lastParkingDate">Specify lastParkingDate (date upto which we need to minimize transactions</param>
        /// <response code="200">Successfully loaded the paid parkings</response>
        /// <response code="204">Content not available</response>
        /// <response code="500">Internal server error</response>
        /// <returns>BaseResponse</returns>
        [HttpGet("ReduceTransactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReduceTransactionsAsync([FromQuery] DateTime lastParkingDate)
        {
            BaseResponse response = await _service.ReduceTransactionsAsync(lastParkingDate);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function minimizes transactions
        /// </summary>
        /// <response code="200">Successfully loaded the paid parkings</response>
        /// <response code="204">Content not available</response>
        /// <response code="412">User not exist</response>
        /// <response code="500">Internal server error</response>
        /// <returns>BaseResponse</returns>
        [HttpGet("Dues")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<SettleBalanceDTO>>> CalculateDuesAsync()
        {
            var emailId = AuthenticationBearer.GetEmail(User);
            BaseResponse<List<SettleBalanceDTO>> response = await _service.CalculateDuesAsync(emailId);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function minimizes transactions
        /// </summary>
        /// <param name="lastParkingDate">Specify lastParkingDate (date upto which we need to minimize transactions</param>
        /// <response code="201">Successfully loaded the paid parkings</response>
        /// <response code="204">Content not available</response>
        /// <response code="500">Internal server error</response>
        /// <returns>BaseResponse</returns>
        [HttpPatch("SettleBalance")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaidParkingDTO>> SettleBalanceAsync(SettleBalanceDTO balanceDTO)
        {
            BaseResponse<PaidParkingDTO> response = await _service.SettleBalanceAsync(balanceDTO);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function deletes paid parking by given id
        /// </summary>
        /// <param name="id">Specify paid parking id</param>
        /// <returns>BaseResponse object</returns>        
        /// <response code="200">Successfully deleted the paid parking</response>      
        /// <response code="404">PaidParking not found</response>
        /// <response code="412">Cannot use someone else paid parking</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePaidParkingByIdAsync([FromRoute] int id)
        {
            var emailId = AuthenticationBearer.GetEmail(User);
            BaseResponse response = await _service.DeletePaidParkingByIdAsync(id, emailId, User);
            return ReplyBaseResponse(response);
        }
    }
}
