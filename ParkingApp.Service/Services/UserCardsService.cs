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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Services
{
    /// <summary>
    /// UserCardsService
    /// </summary>
    public class UserCardsService
    {
        public static bool _automationRunning = false;
        private readonly UserCardsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ParkingAppUtility _utility;

        /// <summary>
        /// Class constructor
        /// </summary>
        public UserCardsService(UserCardsRepository repository, IMapper mapper, ParkingAppUtility utility)
        {
            _repository = repository;
            _mapper = mapper;
            _utility = utility;
        }

        /// <summary>
        /// To get all user cards from database
        /// </summary>
        /// <param name="query">Specify query</param>
        /// <returns>List of UserCardDTO</returns>
        public async Task<BaseResponse<IList<UserCardDTO>>> GetAllUserCardsAsync(Query query)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<IList<UserCardDTO>>(async () =>
            {
                PropertyMapper<UserCardDTO, UserCard>.UpdateQueryWithDBOFields(query, _mapper);
                IList<UserCard> response = await _repository.GetAllUserCardsAsync(query);
                if (response.Count > 0)
                {
                    IList<UserCardDTO> mappedResponse = _mapper.Map<IList<UserCard>, IList<UserCardDTO>>(response);
                    foreach (var userCard in mappedResponse)
                    {
                        var carNumber = await _utility.GetCarNumberByUserIdAsync(userCard.UserId);
                        userCard.CarNumber = carNumber;
                    }
                    return new BaseResponse<IList<UserCardDTO>>(mappedResponse);
                }
                return new BaseResponse<IList<UserCardDTO>>(ParkingAppConstants.NoContentAvailable, StatusCodes.Status204NoContent);
            }, ParkingAppConstants.FailedToGetUserCardList);
        }

        /// <summary>
        /// To add new user card record to database
        /// </summary>
        /// <param name="createUserCardsRequest"></param>
        /// <returns>UserCardDTO</returns>
        public async Task<BaseResponse<UserCardDTO>> AddUserCardsAsync(CreateUserCardDTO createUserCardsRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (!_automationRunning)
                {
                    if (createUserCardsRequest == null)
                        return new BaseResponse<UserCardDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);
                    if (createUserCardsRequest.Date.Date == DateTime.Today || createUserCardsRequest.Date.Date > DateTime.Today)
                    {
                        if (!await _utility.CheckUserIdExistsAsync(createUserCardsRequest.UserId))
                            return new BaseResponse<UserCardDTO>(ParkingAppConstants.UserNotExist, StatusCodes.Status412PreconditionFailed);

                        if (!await _utility.CheckCardIdExistsAsync(createUserCardsRequest.CardId))
                            return new BaseResponse<UserCardDTO>(ParkingAppConstants.CardNotExist, StatusCodes.Status412PreconditionFailed);

                        if(await _utility.CheckCardExpiredAsync(createUserCardsRequest.CardId))
                            return new BaseResponse<UserCardDTO>(ParkingAppConstants.CardExpired,StatusCodes.Status412PreconditionFailed);

                        if (await _utility.CheckCardInUseCurrentlyAsync(createUserCardsRequest.CardId))
                            return new BaseResponse<UserCardDTO>(ParkingAppConstants.CardIsInUse, StatusCodes.Status412PreconditionFailed);

                        if (createUserCardsRequest.Date.Date == DateTime.Today)
                        {
                            var cardAlreadyExist = await _repository.GetCardUserAsync(createUserCardsRequest.UserId);
                            if (cardAlreadyExist != null)
                                return new BaseResponse<UserCardDTO>(ParkingAppConstants.AlreadyAssignedCard, StatusCodes.Status412PreconditionFailed);
                        }
                        var UserCardToAdd = _mapper.Map<CreateUserCardDTO, UserCard>(createUserCardsRequest);
                        UserCard response = await _repository.AddUserCardAsync(UserCardToAdd);
                        UserCardDTO mappedResponse = _mapper.Map<UserCard, UserCardDTO>(response);
                        return new BaseResponse<UserCardDTO>(mappedResponse);
                    }
                    return new BaseResponse<UserCardDTO>(ParkingAppConstants.InvalidDate, StatusCodes.Status412PreconditionFailed);
                }
                return new BaseResponse<UserCardDTO>(ParkingAppConstants.RestrictAdd, StatusCodes.Status412PreconditionFailed);
            }, ParkingAppConstants.FailedToAddUserCard);
        }

        /// <summary>
        /// TO get user card by id
        /// </summary>
        /// <param name="id">Specify id of user card</param>
        /// <returns>UserCardDTO</returns>
        public async Task<BaseResponse<UserCardDTO>> GetUserCardsByIdAsync(int id)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<UserCardDTO>(async () =>
            {
                UserCard result = await _repository.GetUserCardByIdAsync(id);
                if (result != null)
                {
                    var carNumber = await _utility.GetCarNumberByUserIdAsync(result.UserId);
                    var response = _mapper.Map<UserCard, UserCardDTO>(result);
                    response.CarNumber = carNumber;
                    return new BaseResponse<UserCardDTO>(response);
                }
                return new BaseResponse<UserCardDTO>(ParkingAppConstants.UserCardNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToGetUserCard);
        }

        /// <summary>
        /// To update wxisting user card record in database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="updateUserCardsRequest">Specify updateUserCardsRequest</param>
        /// <returns>UserCardDTO</returns>
        public async Task<BaseResponse<UserCardDTO>> UpdateUserCardsAsync(int id, CreateUserCardDTO updateUserCardsRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (updateUserCardsRequest == null)
                    return new BaseResponse<UserCardDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);
                if (updateUserCardsRequest.Date.Date == DateTime.Today || updateUserCardsRequest.Date.Date > DateTime.Today)
                {
                    if (!await _utility.CheckUserIdExistsAsync(updateUserCardsRequest.UserId))
                        return new BaseResponse<UserCardDTO>(ParkingAppConstants.UserNotExist, StatusCodes.Status412PreconditionFailed);

                    if (!await _utility.CheckCardIdExistsAsync(updateUserCardsRequest.CardId))
                        return new BaseResponse<UserCardDTO>(ParkingAppConstants.CardNotExist, StatusCodes.Status412PreconditionFailed);

                    if (updateUserCardsRequest.Date.Date == DateTime.Today)
                    {
                        var cardAlreadyExist = await _repository.GetCardUserAsync(updateUserCardsRequest.UserId);
                        if (cardAlreadyExist != null)
                            return new BaseResponse<UserCardDTO>(ParkingAppConstants.AlreadyAssignedCard, StatusCodes.Status412PreconditionFailed);
                    }
                    var UserCardToUpdate = _mapper.Map<CreateUserCardDTO, UserCard>(updateUserCardsRequest);
                    UserCard response = await _repository.UpdateUserCardAsync(id, UserCardToUpdate);
                    if (response != null)
                    {
                        UserCardDTO mappedResponse = _mapper.Map<UserCard, UserCardDTO>(response);
                        return new BaseResponse<UserCardDTO>(mappedResponse);
                    }
                    return new BaseResponse<UserCardDTO>(ParkingAppConstants.UserCardNotFound, StatusCodes.Status404NotFound);
                }
                return new BaseResponse<UserCardDTO>(ParkingAppConstants.InvalidDate, StatusCodes.Status412PreconditionFailed);
            }, ParkingAppConstants.FailedToUpdateUserCard);
        }

        /// <summary>
        /// To delete existing user card record from database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="emailId">Specify emailId</param>
        /// <param name="userClaim">Specify ClaimsPrincipal</param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteUserCardsByIdAsync(int id, string emailId, ClaimsPrincipal userClaim)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                var userId = await _utility.GetUserIdFromEmail(emailId);
                var existingUserCard = await _repository.GetUserCardByIdAsync(id);
                if (existingUserCard != null)
                {
                    if (existingUserCard.UserId != userId && _utility.GetClaimValue(userClaim, "role") == "User")
                    {
                        return new BaseResponse(ParkingAppConstants.CannotDeleteUserCard, StatusCodes.Status412PreconditionFailed);
                    }
                    var response = await _repository.DeleteUserCardByIdAsync(existingUserCard);
                    return new BaseResponse();
                }
                return new BaseResponse(ParkingAppConstants.UserCardNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToDeleteUserCard);
        }

        /// Still can be improved and enhanced (User validations, desired date, store user attendance sheet in database etc.)
        /// <summary>
        /// To automate card assigning to user 
        /// </summary>
        /// <param name="userAttendances">Specify userAttendances</param>
        /// <param name="isWeeklyBasis">Specify isWeeklyBasis</param>
        /// <param name="duration">Specify duration</param>
        /// <returns>List of user cards</returns>
        public async Task<BaseResponse<List<UserCardDTO>>> AutomateUserCardsAsync(List<UserAttendanceDTO> userAttendances, bool? isWeeklyBasis, int? duration)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (isWeeklyBasis == null || duration == null)
                    return new BaseResponse<List<UserCardDTO>>(ParkingAppConstants.BadRequest, StatusCodes.Status400BadRequest);
                _automationRunning = true;
                List<UserCard> userCards = new List<UserCard>();
                var date = DateTime.UtcNow;
                Dictionary<int, List<DateTime>> userAttendanceDict = new Dictionary<int, List<DateTime>>();

                userAttendanceDict = userAttendances.ToDictionary(x => x.UserId, x => userAttendances.Where(y => y.UserId == x.UserId).Select(z => z.Days).FirstOrDefault());

                for (var i = DateTime.UtcNow; i <= DateTime.UtcNow.AddDays(duration.Value); i = i.AddDays(7))
                {
                    int day = 0;
                    var t = userAttendanceDict.Select(x => new
                    {
                        key = x.Key,
                        value = x.Value.Where(y => _utility.AreWeeksEqual(y, i)).Select(x => x.Date).ToList()
                    }).OrderBy(x => x.value.Count()).ToList();
                    while (day < 7)
                    {
                        // if someone might assign himself card on future date so need to check available card for each date
                        var totalCards = await _utility.GetAvailableCardsAsync(i.AddDays(day).Date);
                        for (int j = 1; j <= totalCards.Count; j++)
                        {
                            List<int> alreadyAssignedCards = await _utility.GetUserCardAsync(i.AddDays(day).Date);
                            var userCount = t.Where(x => !alreadyAssignedCards.Any(y => y == x.key) && x.value.Contains(i.AddDays(day).Date)).ToList();
                            if ((j - 1) < userCount.Count())
                            {
                                // as sequence is in ascending order so first will be the one who is comming for more days
                                userCards.Add(new UserCard
                                {
                                    CardId = totalCards[j - 1].Id,
                                    Date = i.AddDays(day),
                                    UserId = userCount[j - 1].key
                                });
                                t[j - 1].value.Remove(i.AddDays(day));   // assign card from top as users are arranged in ascending order and remove that
                            }
                            continue;
                        }
                        if (!isWeeklyBasis.Value)
                            t = t.OrderByDescending(x => x.value.Count()).ToList();
                        day++;
                    }
                    if (isWeeklyBasis.Value)
                        t = t.OrderByDescending(x => x.value.Count()).ToList();
                }
                var response = await _repository.AddUserCardsInBulkAsync(userCards);
                _automationRunning = false;
                return new BaseResponse<List<UserCardDTO>>(_mapper.Map<List<UserCard>, List<UserCardDTO>>(response));
            }, ParkingAppConstants.FailedToAssignCards);
        }
    }
}
