using AutoMapper;
using Lisec.Base.Utilities.ResponseUtilities;
using Lisec.ParkingApp.DTOs;
using Lisec.ParkingApp.Models;
using Lisec.ParkingApp.Repositories;
using Lisec.ParkingApp.Utilities;
using Lisec.ServiceBase.QueryFilter;
using Lisec.ServiceBase.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Services
{
    /// <summary>
    /// CardsService
    /// </summary>
    public class CardsService
    {
        private readonly CardsRepository _repository;
        private readonly ParkingAppUtility _utility;
        private readonly IMapper _mapper;

        /// <summary>
        /// Class constructor
        /// </summary>
        public CardsService(CardsRepository repository, IMapper mapper, ParkingAppUtility utility)
        {
            _repository = repository;
            _mapper = mapper;
            _utility = utility;
        }

        /// <summary>
        /// To get all cards from database
        /// </summary>
        /// <param name="query">Specifty query</param>
        /// <returns>List of cards</returns>
        public async Task<BaseResponse<IList<CardDTO>>> GetAllCardsAsync(Query query)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<IList<CardDTO>>(async () =>
            {
                PropertyMapper<CardDTO, Card>.UpdateQueryWithDBOFields(query, _mapper);
                IList<Card> response = await _repository.GetAllCardsAsync(query);
                if (response.Count > 0)
                {
                    IList<CardDTO> mappedResponse = _mapper.Map<IList<Card>, IList<CardDTO>>(response);
                    return new BaseResponse<IList<CardDTO>>(mappedResponse);
                }
                return new BaseResponse<IList<CardDTO>>(ParkingAppConstants.NoContentAvailable, StatusCodes.Status204NoContent);
            }, ParkingAppConstants.FailedToGetCardList);
        }

        /// <summary>
        /// TO get card by given id
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns></returns>
        public async Task<BaseResponse<CardDTO>> GetCardByIdAsync(int id)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<CardDTO>(async () =>
            {
                var result = await _repository.GetCardByIdAsync(id);
                if (result != null)
                {
                    var response = _mapper.Map<Card, CardDTO>(result);
                    return new BaseResponse<CardDTO>(response);
                }
                return new BaseResponse<CardDTO>(ParkingAppConstants.CardNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToGetCard);
        }

        /// <summary>
        /// To get all the available cards
        /// </summary>
        /// <param name="date">Specify date</param>
        /// <returns>List of card dto</returns>
        public async Task<BaseResponse<IList<CardDTO>>> GetAvailableCardsAsync(DateTime date)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<IList<CardDTO>>(async () =>
            {
                var result = await _repository.GetAvailableCardsAsync(date);
                if (result.Count > 0)
                {
                    IList<CardDTO> mappedResponse = _mapper.Map<IList<Card>, IList<CardDTO>>(result);
                    return new BaseResponse<IList<CardDTO>>(mappedResponse);
                }
                return new BaseResponse<IList<CardDTO>>(ParkingAppConstants.NoCardAvailable, StatusCodes.Status204NoContent);
            }, ParkingAppConstants.FailedToGetAvailableCardList);
        }

        /// <summary>
        /// To add new card to database
        /// </summary>
        /// <param name="createCardRequest">Specify createCardRequest</param>
        /// <returns>CreateCardDTO</returns>
        public async Task<BaseResponse<CardDTO>> AddCardAsync(CreateCardDTO createCardRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (createCardRequest == null)
                    return new BaseResponse<CardDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);
                if (await _repository.CheckCardAlreadyExistsAsync(createCardRequest.CardNumber))
                    return new BaseResponse<CardDTO>(ParkingAppConstants.CardAlreadyExist, StatusCodes.Status412PreconditionFailed);
                var cardToAdd = _mapper.Map<CreateCardDTO, Card>(createCardRequest);
                Card response = await _repository.AddCardAsync(cardToAdd);
                CardDTO mappedResponse = _mapper.Map<Card, CardDTO>(response);
                return new BaseResponse<CardDTO>(mappedResponse);

            }, ParkingAppConstants.FailedToAddCard);
        }

        /// <summary>
        /// To update existing card in database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="updateCardRequest">Specify updateCardRequest</param>
        /// <returns>CreateCardDTO</returns>
        public async Task<BaseResponse<CardDTO>> UpdateCardAsync(int id, CreateCardDTO updateCardRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (updateCardRequest == null)
                    return new BaseResponse<CardDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);

                if (await _repository.CheckCardAlreadyExistsAsync(updateCardRequest.CardNumber, true,id))
                    return new BaseResponse<CardDTO>(ParkingAppConstants.CardAlreadyExist, StatusCodes.Status412PreconditionFailed);
                var cardToUpdate = _mapper.Map<CreateCardDTO, Card>(updateCardRequest);
                Card response = await _repository.UpdateCardAsync(id, cardToUpdate);
                if (response != null)
                {
                    CardDTO mappedResponse = _mapper.Map<Card, CardDTO>(response);
                    return new BaseResponse<CardDTO>(mappedResponse);
                }
                return new BaseResponse<CardDTO>(ParkingAppConstants.CardNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToUpdateCard);
        }

        // TODO delete all the records
        /// <summary>
        /// To delete the existing card from database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteCardByIdAsync(int id)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (await _utility.CheckCardInUseCurrentlyAsync(id))
                    return new BaseResponse(ParkingAppConstants.CardIsInUse, StatusCodes.Status412PreconditionFailed);

                var response = await _repository.DeleteCardByIdAsync(id);
                if (response != null)
                {
                    return new BaseResponse();
                }
                return new BaseResponse(ParkingAppConstants.CardNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToDeleteCard);
        }
    }
}