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
    /// UserCarsService
    /// </summary>
    public class UserCarsService
    {
        private readonly UserCarsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ParkingAppUtility _utility;

        /// <summary>
        /// Class constructor
        /// </summary>
        public UserCarsService(UserCarsRepository repository, IMapper mapper, ParkingAppUtility utility)
        {
            _repository = repository;
            _mapper = mapper;
            _utility = utility;
        }

        /// <summary>
        /// To get all user cars from database
        /// </summary>
        /// <param name="query">Specify query</param>
        /// <returns>List of UserCarDTO</returns>
        public async Task<BaseResponse<IList<UserCarDTO>>> GetAllUserCarsAsync(Query query)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<IList<UserCarDTO>>(async () =>
            {
                PropertyMapper<UserCarDTO, UserCar>.UpdateQueryWithDBOFields(query, _mapper);
                IList<UserCar> response = await _repository.GetAllUserCarsAsync(query);
                if (response.Count > 0)
                {
                    IList<UserCarDTO> mappedResponse = _mapper.Map<IList<UserCar>, IList<UserCarDTO>>(response);
                    return new BaseResponse<IList<UserCarDTO>>(mappedResponse);
                }
                return new BaseResponse<IList<UserCarDTO>>(ParkingAppConstants.NoContentAvailable, StatusCodes.Status204NoContent);
            }, ParkingAppConstants.FailedToGetUserCarList);
        }

        /// <summary>
        /// To get user car by id 
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns>UserCarDTO</returns>
        public async Task<BaseResponse<UserCarDTO>> GetUserCarByIdAsync(int id)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<UserCarDTO>(async () =>
            {
                var result = await _repository.GetUserCarByIdAsync(id);
                if (result != null)
                {
                    var response = _mapper.Map<UserCar, UserCarDTO>(result);
                    return new BaseResponse<UserCarDTO>(response);
                }
                return new BaseResponse<UserCarDTO>(ParkingAppConstants.UserCarNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToGetUserCar);
        }

        /// <summary>
        /// To add new user car record to database
        /// </summary>
        /// <param name="createUserCarRequest">Specify createUserCarRequest</param>
        /// <returns>UserCarDTO</returns>
        public async Task<BaseResponse<UserCarDTO>> AddUserCarAsync(CreateUserCarDTO createUserCarRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (createUserCarRequest == null)
                    return new BaseResponse<UserCarDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);

                if (await _repository.CheckCarAlreadyRegistered(createUserCarRequest.CarNumber))
                    return new BaseResponse<UserCarDTO>(ParkingAppConstants.CarAlreadyRegistered, StatusCodes.Status412PreconditionFailed);

                if (!await _utility.CheckUserIdExistsAsync(createUserCarRequest.UserId))
                    return new BaseResponse<UserCarDTO>(ParkingAppConstants.UserNotExist, StatusCodes.Status412PreconditionFailed);

                var UserCarToAdd = _mapper.Map<CreateUserCarDTO, UserCar>(createUserCarRequest);
                UserCar response = await _repository.AddUserCarAsync(UserCarToAdd);
                UserCarDTO mappedResponse = _mapper.Map<UserCar, UserCarDTO>(response);
                return new BaseResponse<UserCarDTO>(mappedResponse);

            }, ParkingAppConstants.FailedToAddUserCar);
        }

        /// <summary>
        /// To update existing user car record
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="updateUserCarRequest">Specify updateUserCarRequest</param>
        /// <returns>UserCarDTO</returns>
        public async Task<BaseResponse<UserCarDTO>> UpdateUserCarAsync(int id, CreateUserCarDTO updateUserCarRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (updateUserCarRequest == null)
                    return new BaseResponse<UserCarDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);

                if (await _repository.CheckCarAlreadyRegistered(updateUserCarRequest.CarNumber))
                    return new BaseResponse<UserCarDTO>(ParkingAppConstants.CarAlreadyRegistered, StatusCodes.Status412PreconditionFailed);

                if (!await _utility.CheckUserIdExistsAsync(updateUserCarRequest.UserId))
                    return new BaseResponse<UserCarDTO>(ParkingAppConstants.UserNotExist, StatusCodes.Status412PreconditionFailed);

                var UserCarToUpdate = _mapper.Map<CreateUserCarDTO, UserCar>(updateUserCarRequest);
                UserCar response = await _repository.UpdateUserCarAsync(id, UserCarToUpdate);
                if (response != null)
                {
                    UserCarDTO mappedResponse = _mapper.Map<UserCar, UserCarDTO>(response);
                    return new BaseResponse<UserCarDTO>(mappedResponse);
                }
                return new BaseResponse<UserCarDTO>(ParkingAppConstants.UserCarNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToUpdateUserCar);
        }

        /// <summary>
        /// To delete user car from database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteUserCarByIdAsync(int id)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {

                var response = await _repository.DeleteUserCarByIdAsync(id);
                if (response != null)
                {
                    return new BaseResponse();
                }
                return new BaseResponse(ParkingAppConstants.UserCarNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToDeleteUserCar);
        }
    }
}
