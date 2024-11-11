using Lisec.Base.Utilities.ResponseUtilities;
using Lisec.ParkingApp.DTOs;
using Lisec.ParkingApp.Services;
using Lisec.ServiceBase.Authentication;
using Lisec.ServiceBase.Controllers;
using Lisec.ServiceBase.QueryFilter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Controllers
{
    /// <summary>
    /// UserCardsController
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UserCardsController : BaseController
    {
        private readonly UserCardsService _service;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="service">Specify UserCardsService</param>
        public UserCardsController(UserCardsService service)
        {
            _service = service;
        }

        /// <summary>
        /// This function gets list of all user cards from database
        /// </summary>
        /// <param name="query">Specify query</param>
        /// <response code="200">Successfully loaded the user cards</response>
        /// <response code="204">Content not available</response>
        /// <response code="500">Internal server error</response>
        /// <returns>BaseResponse</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<UserCardDTO>>> GetAllUserCardsAsync([FromQuery] Query query = null)
        {
            BaseResponse<IList<UserCardDTO>> response = await _service.GetAllUserCardsAsync(query);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function get user cards by id
        /// </summary>
        /// <param name="id">Specify user cards id</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully loaded the user cards</response>
        /// <response code="404">cardsd not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserCardDTO>> GetUserCardsByIdAsync([FromRoute] int id)
        {
            BaseResponse<UserCardDTO> response = await _service.GetUserCardsByIdAsync(id);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function adds user cards record in Database
        /// </summary>
        /// <param name="createUsercardsRequest">Object of create and update user cards</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="201">Successfully added the user cards record</response>
        /// <response code="400">BadRequest</response>
        /// <response code="412">USer not exist</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserCardDTO>> AddUserCardsAsync([FromBody] CreateUserCardDTO createUserCardsRequest)
        {
            BaseResponse<UserCardDTO> response = await _service.AddUserCardsAsync(createUserCardsRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function updates user cards with given object
        /// </summary>
        /// <param name="id">Specify user cards id</param>
        /// <param name="updateUsercardsRequest">Object of update user cards</param>        
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully updated the user cards</response>
        /// <response code="400">BadRequest</response>
        /// <response code="404">User cards not found</response>
        /// /// <response code="412">User not exist</response>
        /// <response code="409">Conflict error</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserCardDTO>> UpdateUserCardsAsync([FromRoute] int id, [FromBody] CreateUserCardDTO updateUserCardsRequest)
        {
            BaseResponse<UserCardDTO> response = await _service.UpdateUserCardsAsync(id, updateUserCardsRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function assign cards automatically to all users
        /// </summary>
        /// <param name="isWeeklyBasis">Assign card on weekly basis or not</param>
        /// <param name="userAttendances">List of user attending days</param>   
        /// <param name="duration">Specify durations</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully updated the user cards</response>
        /// <response code="400">BadRequest</response>
        /// <response code="409">Conflict error</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("AutomateCardAssign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<UserCardDTO>>> AutomaticAssignUserCardsAsync([FromBody] List<UserAttendanceDTO> userAttendances, [FromQuery] bool? isWeeklyBasis, [FromQuery] int? duration)
        {
            BaseResponse<List<UserCardDTO>> response = await _service.AutomateUserCardsAsync(userAttendances, isWeeklyBasis, duration);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function deletes user cards by given id
        /// </summary>
        /// <param name="id">Specify user cards id</param>
        /// <returns>BaseResponse object</returns>        
        /// <response code="200">Successfully deleted the user cards</response>      
        /// <response code="404">User cards not found</response>
        /// <response code="412">Cannot drop card assigned to someone else</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserCardsByIdAsync([FromRoute] int id)
        {
            var emailId = AuthenticationBearer.GetEmail(User);
            BaseResponse response = await _service.DeleteUserCardsByIdAsync(id, emailId, User);
            return ReplyBaseResponse(response);
        }
    }
}
