using Lisec.Base.Utilities.ResponseUtilities;
using Lisec.ParkingApp.DTOs;
using Lisec.ParkingApp.Services;
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
    /// CardsController
    /// </summary>
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CardsController : BaseController
    {
        private readonly CardsService _service;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="service">Specify CardsService</param>
        public CardsController(CardsService service)
        {
            _service = service;
        }

        /// <summary>
        /// This function gets list of all cards from database
        /// </summary>
        /// <param name="query">Specify query</param>
        /// <response code="200">Successfully loaded the cards</response>
        /// <response code="204">Content not available</response>
        /// <response code="500">Internal server error</response>
        /// <returns>BaseResponse</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<CardDTO>>> GetAllCardsAsync([FromQuery] Query query = null)
        {
            BaseResponse<IList<CardDTO>> response = await _service.GetAllCardsAsync(query);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function gets list of all cards from database
        /// </summary>
        /// <param name="query">Specify query</param>
        /// <response code="200">Successfully loaded the cards</response>
        /// <response code="204">Content not available</response>
        /// <response code="500">Internal server error</response>
        /// <returns>BaseResponse</returns>
        [HttpGet("AvailableCards")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<CardDTO>>> GetAvailableCardsAsync([FromQuery] DateTime date)
        {
            BaseResponse<IList<CardDTO>> response = await _service.GetAvailableCardsAsync(date);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function get card by id
        /// </summary>
        /// <param name="id">Specify card id</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully loaded the card</response>
        /// <response code="404">Card not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CardDTO>> GetCardsByIdAsync([FromRoute] int id)
        {
            BaseResponse<CardDTO> response = await _service.GetCardByIdAsync(id);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function adds card record in Database
        /// </summary>
        /// <param name="createCardRequest">Object of create and update cards</param>
        /// <returns>BaseResponse object</returns>
        /// <response code="201">Successfully added the cards record</response>
        /// <response code="400">BadRequest</response>
        /// <response code="412">Card with same number already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CardDTO>> AddCardAsync([FromBody] CreateCardDTO createCardRequest)
        {
            BaseResponse<CardDTO> response = await _service.AddCardAsync(createCardRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function updates cards with given object
        /// </summary>
        /// <param name="id">Specify card id</param>
        /// <param name="updateCardRequest">Object of update card</param>        
        /// <returns>BaseResponse object</returns>
        /// <response code="200">Successfully updated the card</response>
        /// <response code="400">BadRequest</response>
        /// <response code="412">Card with same number already exists</response>
        /// <response code="404">Card not found</response>
        /// <response code="409">Conflict error</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CardDTO>> UpdateCardAsync([FromRoute] int id, [FromBody] CreateCardDTO updateCardRequest)
        {
            BaseResponse<CardDTO> response = await _service.UpdateCardAsync(id, updateCardRequest);
            return ReplyBaseResponse(response);
        }

        /// <summary>
        /// This function deletes card by given id
        /// </summary>
        /// <param name="id">Specify card id</param>
        /// <returns>BaseResponse object</returns>        
        /// <response code="200">Successfully deleted the card</response>      
        /// <response code="404">Card not found</response>
        /// <response code="412">Card is currently in use</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCardByIdAsync([FromRoute] int id)
        {
            BaseResponse response = await _service.DeleteCardByIdAsync(id);
            return ReplyBaseResponse(response);
        }
    }
}
