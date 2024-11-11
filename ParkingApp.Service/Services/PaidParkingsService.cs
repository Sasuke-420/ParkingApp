using AutoMapper;
using Lisec.Base.Utilities.ResponseUtilities;
using Lisec.ParkingApp.DTOs;
using Lisec.ParkingApp.Models;
using Lisec.ParkingApp.Repositories;
using Lisec.ParkingApp.Utilities;
using Lisec.ServiceBase.QueryFilter;
using Lisec.ServiceBase.Utilities;
using Lisec.UserManagementDB.Domain.Models.Master;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Services
{
    /// <summary>
    /// PaidParkingsService
    /// </summary>
    public class PaidParkingsService
    {
        private readonly PaidParkingsRepository _repository;
        private readonly ParkingAppUtility _utility;
        private readonly IMapper _mapper;

        /// <summary>
        /// Class constructor
        /// </summary>
        public PaidParkingsService(PaidParkingsRepository repository, IMapper mapper, ParkingAppUtility utility)
        {
            _repository = repository;
            _mapper = mapper;
            _utility = utility;
        }

        /// <summary>
        /// To get all paid parkings from database
        /// </summary>
        /// <param name="query">Specifty query</param>
        /// <returns>List of paid parkings</returns>
        public async Task<BaseResponse<IList<PaidParkingDTO>>> GetAllPaidParkingsAsync(Query query)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<IList<PaidParkingDTO>>(async () =>
            {
                PropertyMapper<PaidParkingDTO, PaidParking>.UpdateQueryWithDBOFields(query, _mapper);
                IList<PaidParking> response = await _repository.GetAllPaidParkingsAsync(query);
                if (response.Count > 0)
                {
                    IList<PaidParkingDTO> mappedResponse = _mapper.Map<IList<PaidParking>, IList<PaidParkingDTO>>(response);
                    return new BaseResponse<IList<PaidParkingDTO>>(mappedResponse);
                }
                return new BaseResponse<IList<PaidParkingDTO>>(ParkingAppConstants.NoContentAvailable, StatusCodes.Status204NoContent);
            }, ParkingAppConstants.FailedToGetPaidParkingList);
        }

        /// <summary>
        /// TO get paid parking by given id
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns></returns>
        public async Task<BaseResponse<PaidParkingDTO>> GetPaidParkingByIdAsync(int id)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<PaidParkingDTO>(async () =>
            {
                var result = await _repository.GetPaidParkingByIdAsync(id);
                if (result != null)
                {
                    var response = _mapper.Map<PaidParking, PaidParkingDTO>(result);
                    return new BaseResponse<PaidParkingDTO>(response);
                }
                return new BaseResponse<PaidParkingDTO>(ParkingAppConstants.PaidParkingNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToGetPaidParking);
        }

        /// <summary>
        /// To add new paid parking to database
        /// </summary>
        /// <param name="createPaidParkingRequest">Specify createPaidParkingRequest</param>
        /// <returns>CreatePaidParkingDTO</returns>
        public async Task<BaseResponse<PaidParkingDTO>> AddPaidParkingAsync(CreatePaidParkingDTO createPaidParkingRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (createPaidParkingRequest == null)
                    return new BaseResponse<PaidParkingDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);

                if (createPaidParkingRequest.SharesId != null && _utility.HasValidUserIds(createPaidParkingRequest.SharesId))
                    return new BaseResponse<PaidParkingDTO>(ParkingAppConstants.UserNotExist, StatusCodes.Status412PreconditionFailed);

                if (createPaidParkingRequest.GroupId != null && !_utility.ValidGroupId(createPaidParkingRequest.GroupId.Value))
                    return new BaseResponse<PaidParkingDTO>(ParkingAppConstants.GroupDoesNotExist, StatusCodes.Status412PreconditionFailed);

                var limitResponse = await CheckIfUserHasExceededDailyRestrictionAsync(createPaidParkingRequest);
                if (!limitResponse.IsSuccessStatusCode())
                {
                    return new BaseResponse<PaidParkingDTO>(limitResponse.Message, limitResponse.StatusCode);
                }
                var paidparkingToAdd = _mapper.Map<CreatePaidParkingDTO, PaidParking>(createPaidParkingRequest);
                if (createPaidParkingRequest.GroupId.HasValue)
                {
                    createPaidParkingRequest.SharesId ??= new List<int>();
                    if (createPaidParkingRequest.SharesId != null)
                        createPaidParkingRequest.SharesId = createPaidParkingRequest.SharesId.Union(await _utility.GetUsersIdByGroupAsync(createPaidParkingRequest.GroupId.Value)).Distinct().ToList();
                }
                paidparkingToAdd.SharesId = String.Join(',', createPaidParkingRequest.SharesId);

                if (string.IsNullOrEmpty(paidparkingToAdd.SharesId))
                    return new BaseResponse<PaidParkingDTO>(ParkingAppConstants.AlreadyAssignedCard, StatusCodes.Status412PreconditionFailed);

                PaidParking response = await _repository.AddPaidParkingAsync(paidparkingToAdd);

                PaidParkingDTO mappedResponse = _mapper.Map<PaidParking, PaidParkingDTO>(response);

                List<MasterUser> users = _utility.GetUserList(paidparkingToAdd.SharesId.Split(',').Select(x => int.Parse(x)).ToList());
                mappedResponse.Users = _mapper.Map<List<MasterUser>, List<UserDTO>>(users);
                return new BaseResponse<PaidParkingDTO>(mappedResponse);

            }, ParkingAppConstants.FailedToAddPaidParking);
        }

        /// <summary>
        /// To check if user has not exceeded daily limit
        /// </summary>
        /// <param name="createPaidParkingRequest">Specify CreatePaidParkingDTO</param>
        /// <returns></returns>
        private async Task<BaseResponse> CheckIfUserHasExceededDailyRestrictionAsync(CreatePaidParkingDTO createPaidParkingRequest)
        {
            double sum = 0;
            var existingExpense = await _repository.GetPaidParkingsOfUserUptoDateAsync(createPaidParkingRequest.Date, createPaidParkingRequest.UserId);
            var restriction = await _utility.GetRestrictionAsync();
            if (existingExpense.Count > 0)
            {
                existingExpense.ForEach(x =>
                {
                    sum += x.AmountPaid;
                });
            }
            if (restriction != null && (createPaidParkingRequest.AmountPaid + sum) > restriction.Amount)
            {
                return new BaseResponse($"you have reached your daily limit of {restriction.Amount}", StatusCodes.Status412PreconditionFailed);
            }
            return new BaseResponse();
        }

        /// <summary>
        /// To update existing paid parking in database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="updatePaidParkingRequest">Specify updatePaidParkingRequest</param>
        /// <returns>CreatePaidParkingDTO</returns>
        public async Task<BaseResponse<PaidParkingDTO>> UpdatePaidParkingAsync(int id, CreatePaidParkingDTO updatePaidParkingRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (updatePaidParkingRequest == null)
                    return new BaseResponse<PaidParkingDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);

                if (updatePaidParkingRequest.SharesId.Any() && _utility.HasValidUserIds(updatePaidParkingRequest.SharesId))
                    return new BaseResponse<PaidParkingDTO>(ParkingAppConstants.UserNotExist, StatusCodes.Status412PreconditionFailed);

                var limitResponse = await CheckIfUserHasExceededDailyRestrictionAsync(updatePaidParkingRequest);
                if (!limitResponse.IsSuccessStatusCode())
                {
                    return new BaseResponse<PaidParkingDTO>(limitResponse.Message, limitResponse.StatusCode);
                }

                var paidParkingToUpdate = _mapper.Map<CreatePaidParkingDTO, PaidParking>(updatePaidParkingRequest);
                if (updatePaidParkingRequest.GroupId.HasValue)
                {
                    updatePaidParkingRequest.SharesId ??= new List<int>();
                    if (updatePaidParkingRequest.SharesId != null)
                        updatePaidParkingRequest.SharesId = updatePaidParkingRequest.SharesId.Union(await _utility.GetUsersIdByGroupAsync(updatePaidParkingRequest.GroupId.Value)).Distinct().ToList();
                }
                paidParkingToUpdate.SharesId = String.Join(',', updatePaidParkingRequest.SharesId);

                if (string.IsNullOrEmpty(paidParkingToUpdate.SharesId))
                    return new BaseResponse<PaidParkingDTO>(ParkingAppConstants.AlreadyAssignedCard, StatusCodes.Status412PreconditionFailed);

                PaidParking response = await _repository.UpdatePaidParkingAsync(id, paidParkingToUpdate);
                if (response != null)
                {
                    PaidParkingDTO mappedResponse = _mapper.Map<PaidParking, PaidParkingDTO>(response);
                    List<MasterUser> users = _utility.GetUserList(updatePaidParkingRequest.SharesId);
                    mappedResponse.Users = _mapper.Map<List<MasterUser>, List<UserDTO>>(users);
                    return new BaseResponse<PaidParkingDTO>(mappedResponse);
                }
                return new BaseResponse<PaidParkingDTO>(ParkingAppConstants.PaidParkingNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToUpdatePaidParking);
        }

        /// <summary>
        /// To calculate dues
        /// </summary>
        /// <param name="emailId">Specify emailId</param>
        /// <returns></returns>
        public async Task<BaseResponse<List<SettleBalanceDTO>>> CalculateDuesAsync(string emailId)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                var id = await _utility.GetUserIdFromEmail(emailId);
                if (id == null)
                    return new BaseResponse<List<SettleBalanceDTO>>(ParkingAppConstants.UserNotExist, StatusCodes.Status412PreconditionFailed);

                List<PaidParking> paidParkingRecords = await _repository.GetDuesAsync(id.Value);
                if (paidParkingRecords == null || !paidParkingRecords.Any())
                    return new BaseResponse<List<SettleBalanceDTO>>(ParkingAppConstants.NoCardAvailable, StatusCodes.Status204NoContent);
                List<SettleBalanceDTO> result = new List<SettleBalanceDTO>();
                var paidParkignDTO = _mapper.Map<List<PaidParking>, List<PaidParkingDTO>>(paidParkingRecords);
                foreach (var record in paidParkignDTO)
                {
                    if (record.UserId != id && record.Users.Any(x => x.Id == id))
                    {
                        var existingDue = result.FirstOrDefault(x => x.PayeeId == record.UserId);
                        if (existingDue == null)
                        {
                            result.Add(new SettleBalanceDTO
                            {
                                PayerId = id.Value,
                                PayeeId = record.UserId,
                                Balance = Math.Round(record.AmountPaid / record.Users.Count(), 2, MidpointRounding.AwayFromZero)
                            });
                        }
                        else
                        {
                            existingDue.Balance += Math.Round(record.AmountPaid / record.Users.Count(), 2, MidpointRounding.AwayFromZero);
                        }

                    }
                    if (record.UserId == id)
                    {
                        foreach (var user in record.Users)
                        {
                            if (id == user.Id)
                            {
                                continue;
                            }
                            var existingDue = result.FirstOrDefault(x => x.PayerId == user.Id);
                            if (existingDue == null)
                            {
                                result.Add(new SettleBalanceDTO
                                {
                                    PayerId = user.Id,
                                    PayeeId = id.Value,
                                    Balance = Math.Round(record.AmountPaid / record.Users.Count(), 2, MidpointRounding.AwayFromZero)
                                });
                            }
                            else
                            {
                                existingDue.Balance += Math.Round(record.AmountPaid / record.Users.Count(), 2, MidpointRounding.AwayFromZero);
                            }
                        }
                    }
                }
                return new BaseResponse<List<SettleBalanceDTO>>(result);
            }, ParkingAppConstants.FailedToCalculateDues);
        }

        /// <summary>
        /// To settlle balance
        /// </summary>
        /// <param name="balanceDTO">Specify balanceDTO</param>
        /// <returns></returns>
        public async Task<BaseResponse<PaidParkingDTO>> SettleBalanceAsync(SettleBalanceDTO balanceDTO)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (!await _utility.CheckUserIdExistsAsync(balanceDTO.PayeeId))
                    return new BaseResponse<PaidParkingDTO>(ParkingAppConstants.PayeeNotExist, StatusCodes.Status412PreconditionFailed);

                if (!await _utility.CheckUserIdExistsAsync(balanceDTO.PayerId))
                    return new BaseResponse<PaidParkingDTO>(ParkingAppConstants.PayerNotExist, StatusCodes.Status412PreconditionFailed);
                List<PaidParking> paidParkingRecords = await _repository.GetBalanceAsync(balanceDTO.PayeeId, balanceDTO.PayerId);

                if (paidParkingRecords == null || paidParkingRecords.Count() == 0)
                    return new BaseResponse<PaidParkingDTO>(ParkingAppConstants.NoDebtPending, StatusCodes.Status204NoContent);

                var paidParkignDTOList = _mapper.Map<List<PaidParking>, List<PaidParkingDTO>>(paidParkingRecords);
                await SettleBalanceAsync(paidParkignDTOList, balanceDTO);
                return new BaseResponse<PaidParkingDTO>();
            }, ParkingAppConstants.FailedToUpdatePaidParking);
        }

        // TODO need to use settled flag here to keep track of history 
        /// <summary>
        /// TO settle the balance
        /// </summary>
        /// <param name="paidParkignDTOList">Specify paidParkingDTO list</param>
        /// <param name="balanceDTO">Specify balanceDTO</param>
        /// <returns></returns>
        private async Task SettleBalanceAsync(List<PaidParkingDTO> paidParkignDTOList, SettleBalanceDTO balanceDTO)
        {
            double amount = balanceDTO.Balance;
            foreach (var paidParking in paidParkignDTOList)
            {
                if (amount >= 0.10)
                {
                    var share = Math.Round(paidParking.AmountPaid / paidParking.Users.Count(), 2, MidpointRounding.AwayFromZero);
                    double diff = amount - share;
                    if (diff > 0.10)
                    {
                        amount = diff;
                        await _repository.DeletePaidParkingByIdAsync(paidParking.Id);
                        var user = paidParking.Users.Where(x => x.Id == balanceDTO.PayerId).FirstOrDefault();
                        paidParking.Users.Remove(user);
                        if (paidParking.Users.Count() > 0 && paidParking.AmountPaid > 0.10)
                        {
                            await _repository.AddPaidParkingAsync(new PaidParking
                            {
                                AmountPaid = paidParking.AmountPaid - share,
                                Date = DateTime.Now,
                                Settled = false,
                                SharesId = string.Join(',', paidParking.Users.Select(x => x.Id)),
                                UserId = balanceDTO.PayeeId,
                            });
                        }
                    }
                    else
                    {
                        var remainder = share - amount;
                        amount = 0.0;
                        await _repository.DeletePaidParkingByIdAsync(paidParking.Id);
                        var user = paidParking.Users.Where(x => x.Id == balanceDTO.PayerId).FirstOrDefault();
                        paidParking.Users.Remove(user);
                        if (paidParking.Users.Count() > 0 && paidParking.AmountPaid > 0.10)
                        {
                            await _repository.AddPaidParkingAsync(new PaidParking
                            {
                                AmountPaid = paidParking.AmountPaid - share,
                                Date = DateTime.Now,
                                Settled = false,
                                SharesId = string.Join(',', paidParking.Users.Select(x => x.Id)),
                                UserId = balanceDTO.PayeeId,
                            });
                        }
                        if (remainder > 0.10)
                        {
                            await _repository.AddPaidParkingAsync(new PaidParking
                            {
                                AmountPaid = remainder,
                                Date = DateTime.Now,
                                Settled = false,
                                SharesId = balanceDTO.PayerId.ToString(),
                                UserId = balanceDTO.PayeeId,
                            });
                        }
                    }
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// To delete the existing paid parking from database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="emailId">Specify email id</param>
        /// <param name="userClaim">Specify ClaimsPrincipal</param>
        /// <returns></returns>
        public async Task<BaseResponse> DeletePaidParkingByIdAsync(int id, string emailId, ClaimsPrincipal userClaim)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                var userId = await _utility.GetUserIdFromEmail(emailId);
                var existingUser = await _repository.GetPaidParkingByIdAsync(id);
                if (existingUser != null)
                {
                    if (existingUser.UserId != userId && _utility.GetClaimValue(userClaim, "role") == "User")
                    {
                        return new BaseResponse(ParkingAppConstants.CannotDeletePaidParking, StatusCodes.Status412PreconditionFailed);
                    }
                    var response = await _repository.DeletePaidParkingAsync(existingUser);
                }
                return new BaseResponse(ParkingAppConstants.PaidParkingNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToDeletePaidParking);
        }

        // TODO need to use settled flag here to keep track of history 
        /// <summary>
        /// To reduce transactions
        /// </summary>
        /// <param name="lastParkingDate">Specify lastParkingDate</param>
        /// <returns>PaidParkingDTO</returns>
        public async Task<BaseResponse> ReduceTransactionsAsync(DateTime lastParkingDate)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                var paidParkingRecords = await _repository.GetPaidParkingsUptoDateAsync(lastParkingDate);

                if (paidParkingRecords == null || paidParkingRecords.Count() == 0)
                    return new BaseResponse(ParkingAppConstants.NoContentAvailable, StatusCodes.Status204NoContent);

                var paidParkignDTO = _mapper.Map<List<PaidParking>, List<PaidParkingDTO>>(paidParkingRecords);
                List<int> userIds = GetUserIds(paidParkignDTO);
                userIds.Sort();
                double[,] graph = CreateGraph(userIds, paidParkignDTO);
                await MinCashFlowAsync(graph, userIds);
                _repository.RemoveBulk(paidParkingRecords);
                return new BaseResponse();
            }, ParkingAppConstants.FailedToUpdatePaidParking);
        }

        /// <summary>
        /// To create graph of cash flow
        /// </summary>
        /// <param name="userIds">Specify userIds</param>
        /// <param name="paidParkingRecords">Specify paidParkingRecords</param>
        /// <returns></returns>
        private double[,] CreateGraph(List<int> userIds, List<PaidParkingDTO> paidParkingRecords)
        {
            var graph = new double[userIds.Count, userIds.Count];
            foreach (var paidParking in paidParkingRecords)
            {
                var share = Math.Round(paidParking.AmountPaid / paidParking.Users.Count, 2, MidpointRounding.AwayFromZero);
                foreach (var user in paidParking.Users)
                {
                    var i = userIds.FindIndex(0, userIds.Count(), x => x == paidParking.UserId);
                    var j = userIds.FindIndex(0, userIds.Count(), x => x == user.Id);
                    if (i != j)
                        graph[i, j] += share;
                }
            }
            return graph;
        }

        /// <summary>
        /// To get all the user ids from paid parking records
        /// </summary>
        /// <param name="paidParkingRecords">Specify paidParkingRecords</param>
        /// <returns></returns>
        private List<int> GetUserIds(List<PaidParkingDTO> paidParkingRecords)
        {
            var userIDs = new List<int>();
            foreach (var paidParking in paidParkingRecords ?? Enumerable.Empty<PaidParkingDTO>())
            {
                if (!userIDs.Contains(paidParking.UserId))
                {
                    userIDs.Add(paidParking.UserId);
                }
                paidParking.Users.ForEach(x =>
                {
                    if (!userIDs.Contains(x.Id))
                    {
                        userIDs.Add(x.Id);
                    }
                });
            }
            return userIDs;
        }

        /// <summary>
        /// To get index of minimum value in arr[]
        /// </summary>
        /// <param name="arr">Specify arr[]</param>
        /// <param name="count">Specify count</param>
        /// <returns></returns>
        private int GetMin(double[] arr, int count)
        {
            int minInd = 0;
            for (int i = 1; i < count; i++)
                if (arr[i] < arr[minInd])
                    minInd = i;
            return minInd;
        }

        /// <summary>
        /// To get index of maximum value in arr[]
        /// </summary>
        /// <param name="arr">Specify arr[]</param>
        /// <param name="count">Specify count</param>
        /// <returns></returns>
        private int GetMax(double[] arr, int count)
        {
            int maxInd = 0;
            for (int i = 1; i < count; i++)
                if (arr[i] > arr[maxInd])
                    maxInd = i;
            return maxInd;
        }

        /// <summary>
        /// To get minimum of 2 values
        /// </summary>
        /// <param name="x">Specify first double value</param>
        /// <param name="y">Specify second double value</param>
        /// <returns></returns>
        private double MinOf2(double x, double y)
        {
            return (x < y) ? x : y;
        }

        /// <summary>
        /// Recursive function to minimize transactions
        /// </summary>
        /// <param name="amount">Specify amount</param>
        /// <param name="userIds">Specify userIds</param>
        /// <param name="count">Specify count</param>
        /// <returns></returns>
        private async Task MinCashFlowRecAsync(double[] amount, List<int> userIds, int count)
        {
            int mxCredit = GetMax(amount, count), mxDebit = GetMin(amount, count);

            if (Math.Abs(amount[mxCredit]) < 0.10 && Math.Abs(amount[mxDebit]) < 0.10)
                return;

            double min = MinOf2(-amount[mxDebit], amount[mxCredit]);
            amount[mxCredit] -= min;
            amount[mxDebit] += min;

            await _repository.AddPaidParkingAsync(new PaidParking
            {
                AmountPaid = Math.Round(min, 2, MidpointRounding.AwayFromZero),
                UserId = userIds[mxDebit],
                SharesId = userIds[mxCredit].ToString(),
                Settled = false,
                Date = DateTime.Now,
            });
            await MinCashFlowRecAsync(amount, userIds, count);
        }

        /// <summary>
        /// Function to minimize the cash flow
        /// </summary>
        /// <param name="graph">Specify graph</param>
        /// <param name="userIds">Specify list of user ids</param>
        /// <returns></returns>
        private async Task MinCashFlowAsync(double[,] graph, List<int> userIds)
        {
            var count = userIds.Count;
            double[] amount = new double[count];
            if (count > 0)
            {
                for (int p = 0; p < count; p++)
                    for (int i = 0; i < count; i++)
                        amount[p] += (graph[i, p] - graph[p, i]);

                await MinCashFlowRecAsync(amount, userIds, userIds.Count);
            }
        }
    }
}
