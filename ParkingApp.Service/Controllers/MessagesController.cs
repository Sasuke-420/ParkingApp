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
    /// MessagesController
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MessagesController : BaseController
    {
        private readonly MessagesService _service;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="service">Specify MessagesService</param>
        public MessagesController(MessagesService service)
        {
            _service = service;
        }

        /// <summary>
        /// This function gets list of all messages from database
        /// </summary>
        /// <param name="query">Specify query</param>
        /// <response code="200">Successfully loaded the messages</response>
        /// <response code="204">Content not available</response>
        /// <response code="500">Internal server error</response>
        /// <returns>BaseResponse</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<MessageDTO>>> GetAllMessagesAsync([FromQuery] Query query = null)
        {
            BaseResponse<IList<MessageDTO>> response = await _service.GetAllMessagesAsync(query);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function get message by id
        /// </summary>
        /// <param name="id">Specify message id</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully loaded the message</response>
        /// <response code="404">Message not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MessageDTO>> GetMessageByIdAsync([FromRoute] int id)
        {
            BaseResponse<MessageDTO> response = await _service.GetMessageByIdAsync(id);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function adds message record in Database
        /// </summary>
        /// <param name="createMessageRequest">Object of create and update messages</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="201">Successfully added the messages record</response>
        /// <response code="400">BadRequest</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MessageDTO>> AddMessageAsync([FromBody] CreateMessageDTO createMessageRequest)
        {
            BaseResponse<MessageDTO> response = await _service.AddMessageAsync(createMessageRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function updates messages with given object
        /// </summary>
        /// <param name="id">Specify message id</param>
        /// <param name="updateMessageRequest">Object of update message</param>        
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully updated the message</response>
        /// <response code="400">BadRequest</response>
        /// <response code="404">Message not found</response>
        /// <response code="409">Conflict error</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MessageDTO>> UpdateMessageAsync([FromRoute] int id, [FromBody] CreateMessageDTO updateMessageRequest)
        {
            BaseResponse<MessageDTO> response = await _service.UpdateMessageAsync(id, updateMessageRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function deletes message by given id
        /// </summary>
        /// <param name="id">Specify message id</param>
        /// <returns>BaseResponse object</returns>        
        /// <response code="200">Successfully deleted the message</response>      
        /// <response code="404">Message not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMessageByIdAsync([FromRoute] int id)
        {
            BaseResponse response = await _service.DeleteMessageByIdAsync(id);
            return ReplyBaseResponse(response);
        }
    }
}
