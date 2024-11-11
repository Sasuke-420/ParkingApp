using AutoMapper;
using Lisec.Base.Utilities.ResponseUtilities;
using Lisec.ParkingApp.DTOs;
using Lisec.ParkingApp.Models;
using Lisec.ParkingApp.Repositories;
using Lisec.ParkingApp.Utilities;
using Lisec.ServiceBase.QueryFilter;
using Lisec.ServiceBase.Utilities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Services
{
    /// <summary>
    /// MessagesService
    /// </summary>
    public class MessagesService
    {
        private readonly MessagesRepository _repository;
        private readonly ParkingAppUtility _utility;
        private readonly IMapper _mapper;

        /// <summary>
        /// Class constructor
        /// </summary>
        public MessagesService(MessagesRepository repository, IMapper mapper, ParkingAppUtility utility)
        {
            _repository = repository;
            _mapper = mapper;
            _utility = utility;
        }

        /// <summary>
        /// To get all messages from database
        /// </summary>
        /// <param name="query">Specifty query</param>
        /// <returns>List of messages</returns>
        public async Task<BaseResponse<IList<MessageDTO>>> GetAllMessagesAsync(Query query)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<IList<MessageDTO>>(async () =>
            {
                PropertyMapper<MessageDTO, MessageModel>.UpdateQueryWithDBOFields(query, _mapper);
                IList<MessageModel> response = await _repository.GetAllMessagesAsync(query);
                if (response.Count > 0)
                {
                    IList<MessageDTO> mappedResponse = _mapper.Map<IList<MessageModel>, IList<MessageDTO>>(response);
                    return new BaseResponse<IList<MessageDTO>>(mappedResponse);
                }
                return new BaseResponse<IList<MessageDTO>>(ParkingAppConstants.NoContentAvailable, StatusCodes.Status204NoContent);
            }, ParkingAppConstants.FailedToGetMessageList);
        }

        /// <summary>
        /// TO get message by given id
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns></returns>
        public async Task<BaseResponse<MessageDTO>> GetMessageByIdAsync(int id)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<MessageDTO>(async () =>
            {
                var result = await _repository.GetMessageByIdAsync(id);
                if (result != null)
                {
                    var response = _mapper.Map<MessageModel, MessageDTO>(result);
                    return new BaseResponse<MessageDTO>(response);
                }
                return new BaseResponse<MessageDTO>(ParkingAppConstants.MessageNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToGetMessage);
        }

        /// <summary>
        /// To add new message to database
        /// </summary>
        /// <param name="createMessageRequest">Specify createMessageRequest</param>
        /// <returns>CreateMessageDTO</returns>
        public async Task<BaseResponse<MessageDTO>> AddMessageAsync(CreateMessageDTO createMessageRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (createMessageRequest == null)
                    return new BaseResponse<MessageDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);

                if (!await _utility.CheckUserIdExistsAsync(createMessageRequest.UserId))
                    return new BaseResponse<MessageDTO>(ParkingAppConstants.UserNotExist, StatusCodes.Status412PreconditionFailed);

                var messageToAdd = _mapper.Map<CreateMessageDTO, MessageModel>(createMessageRequest);

                MessageModel response = await _repository.AddMessageAsync(messageToAdd);

                MessageDTO mappedResponse = _mapper.Map<MessageModel, MessageDTO>(response);
                return new BaseResponse<MessageDTO>(mappedResponse);

            }, ParkingAppConstants.FailedToAddMessage);
        }

        /// <summary>
        /// To update existing message in database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="updateMessageRequest">Specify updateMessageRequest</param>
        /// <returns>CreateMessageDTO</returns>
        public async Task<BaseResponse<MessageDTO>> UpdateMessageAsync(int id, CreateMessageDTO updateMessageRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (updateMessageRequest == null)
                    return new BaseResponse<MessageDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);

                if (!await _utility.CheckUserIdExistsAsync(updateMessageRequest.UserId))
                    return new BaseResponse<MessageDTO>(ParkingAppConstants.UserNotExist, StatusCodes.Status412PreconditionFailed);

                var messageToUpdate = _mapper.Map<CreateMessageDTO, MessageModel>(updateMessageRequest);
                MessageModel response = await _repository.UpdateMessageAsync(id, messageToUpdate);
                if (response != null)
                {
                    MessageDTO mappedResponse = _mapper.Map<MessageModel, MessageDTO>(response);
                    return new BaseResponse<MessageDTO>(mappedResponse);
                }
                return new BaseResponse<MessageDTO>(ParkingAppConstants.MessageNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToUpdateMessage);
        }

        // TODO delete all the records
        /// <summary>
        /// To delete the existing message from database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteMessageByIdAsync(int id)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                var response = await _repository.DeleteMessageByIdAsync(id);
                if (response != null)
                {
                    return new BaseResponse();
                }
                return new BaseResponse(ParkingAppConstants.MessageNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToDeleteMessage);
        }
    }
}
