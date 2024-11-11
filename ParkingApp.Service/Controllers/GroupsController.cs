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
    /// GroupsController
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class GroupsController : BaseController
    {
        private readonly GroupsService _service;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="service">Specify GroupsService</param>
        public GroupsController(GroupsService service)
        {
            _service = service;
        }

        /// <summary>
        /// This function gets list of all groups from database
        /// </summary>
        /// <param name="query">Specify query</param>
        /// <response code="200">Successfully loaded the groups</response>
        /// <response code="204">Content not available</response>
        /// <response code="500">Internal server error</response>
        /// <returns>BaseResponse</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<GroupDTO>>> GetAllGroupsAsync([FromQuery] Query query = null)
        {
            BaseResponse<IList<GroupDTO>> response = await _service.GetAllGroupsAsync(query);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function get group by id
        /// </summary>
        /// <param name="id">Specify group id</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully loaded the group</response>
        /// <response code="404">Group not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GroupDTO>> GetGroupsByIdAsync([FromRoute] int id)
        {
            BaseResponse<GroupDTO> response = await _service.GetGroupByIdAsync(id);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function adds group record in Database
        /// </summary>
        /// <param name="createGroupRequest">Object of create and update group</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="201">Successfully added the Groups record</response>
        /// <response code="400">BadRequest</response>
        /// <response code="412">Specified user id doesn't exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GroupDTO>> AddGroupAsync([FromBody] CreateGroupDTO createGroupRequest)
        {
            BaseResponse<GroupDTO> response = await _service.AddGroupAsync(createGroupRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function updates groups with given object
        /// </summary>
        /// <param name="id">Specify group id</param>
        /// <param name="updateGroupRequest">Object of update group</param>        
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully updated the group</response>
        /// <response code="400">BadRequest</response>
        /// <response code="404">Group not found</response>
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
        public async Task<ActionResult<GroupDTO>> UpdateGroupAsync([FromRoute] int id, [FromBody] CreateGroupDTO updateGroupRequest)
        {
            BaseResponse<GroupDTO> response = await _service.UpdateGroupAsync(id, updateGroupRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function deletes group by given id
        /// </summary>
        /// <param name="id">Specify group id</param>
        /// <returns>BaseResponse object</returns>        
        /// <response code="200">Successfully deleted the group</response>      
        /// <response code="404">Group not found</response>
        /// <response code="412">Group is currently in use</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteGroupByIdAsync([FromRoute] int id)
        {
            BaseResponse response = await _service.DeleteGroupByIdAsync(id);
            return ReplyBaseResponse(response);
        }
    }
}
