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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Services
{
    /// <summary>
    /// GroupsService
    /// </summary>
    public class GroupsService
    {
        private readonly GroupsRepository _repository;
        private readonly ParkingAppUtility _utility;
        private readonly IMapper _mapper;

        /// <summary>
        /// Class constructor
        /// </summary>
        public GroupsService(GroupsRepository repository, IMapper mapper, ParkingAppUtility utility)
        {
            _repository = repository;
            _mapper = mapper;
            _utility = utility;
        }

        /// <summary>
        /// To get all groups from database
        /// </summary>
        /// <param name="query">Specifty query</param>
        /// <returns>List of groups</returns>
        public async Task<BaseResponse<IList<GroupDTO>>> GetAllGroupsAsync(Query query)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<IList<GroupDTO>>(async () =>
            {
                PropertyMapper<GroupDTO, Group>.UpdateQueryWithDBOFields(query, _mapper);
                IList<Group> response = await _repository.GetAllGroupsAsync(query);
                if (response.Count > 0)
                {
                    IList<GroupDTO> mappedResponse = _mapper.Map<IList<Group>, IList<GroupDTO>>(response);
                    return new BaseResponse<IList<GroupDTO>>(mappedResponse);
                }
                return new BaseResponse<IList<GroupDTO>>(ParkingAppConstants.NoContentAvailable, StatusCodes.Status204NoContent);
            }, ParkingAppConstants.FailedToGetGroupList);
        }

        /// <summary>
        /// TO get group by given id
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns></returns>
        public async Task<BaseResponse<GroupDTO>> GetGroupByIdAsync(int id)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<GroupDTO>(async () =>
            {
                var result = await _repository.GetGroupByIdAsync(id);
                if (result != null)
                {
                    var response = _mapper.Map<Group, GroupDTO>(result);
                    return new BaseResponse<GroupDTO>(response);
                }
                return new BaseResponse<GroupDTO>(ParkingAppConstants.GroupNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToGetGroup);
        }

        /// <summary>
        /// To add new group to database
        /// </summary>
        /// <param name="createGroupRequest">Specify createGroupRequest</param>
        /// <returns>CreateGroupDTO</returns>
        public async Task<BaseResponse<GroupDTO>> AddGroupAsync(CreateGroupDTO createGroupRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (createGroupRequest == null)
                    return new BaseResponse<GroupDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);

                if (createGroupRequest.MemeberIds.Any() && _utility.HasValidUserIds(createGroupRequest.MemeberIds))
                    return new BaseResponse<GroupDTO>(ParkingAppConstants.UserNotExist, StatusCodes.Status412PreconditionFailed);

                var groupToAdd = _mapper.Map<CreateGroupDTO, Group>(createGroupRequest);

                Group response = await _repository.AddGroupAsync(groupToAdd);

                GroupDTO mappedResponse = _mapper.Map<Group, GroupDTO>(response);
                List<MasterUser> users = _utility.GetUserList(mappedResponse.MemeberIds);
                mappedResponse.Users = _mapper.Map<List<MasterUser>, List<UserDTO>>(users);
                return new BaseResponse<GroupDTO>(mappedResponse);

            }, ParkingAppConstants.FailedToAddGroup);
        }

        /// <summary>
        /// To update existing group in database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="updateGroupRequest">Specify updateGroupRequest</param>
        /// <returns>CreateGroupDTO</returns>
        public async Task<BaseResponse<GroupDTO>> UpdateGroupAsync(int id, CreateGroupDTO updateGroupRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (updateGroupRequest == null)
                    return new BaseResponse<GroupDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);

                if (updateGroupRequest.MemeberIds.Any() && _utility.HasValidUserIds(updateGroupRequest.MemeberIds))
                    return new BaseResponse<GroupDTO>(ParkingAppConstants.UserNotExist, StatusCodes.Status412PreconditionFailed);

                var groupToUpdate = _mapper.Map<CreateGroupDTO, Group>(updateGroupRequest);
                Group response = await _repository.UpdateGroupAsync(id, groupToUpdate);
                if (response != null)
                {
                    GroupDTO mappedResponse = _mapper.Map<Group, GroupDTO>(response);
                    List<MasterUser> users = _utility.GetUserList(mappedResponse.MemeberIds);
                    mappedResponse.Users = _mapper.Map<List<MasterUser>, List<UserDTO>>(users);
                    return new BaseResponse<GroupDTO>(mappedResponse);
                }
                return new BaseResponse<GroupDTO>(ParkingAppConstants.GroupNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToUpdateGroup);
        }

        // TODO delete all the records
        /// <summary>
        /// To delete the existing group from database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteGroupByIdAsync(int id)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                var response = await _repository.DeleteGroupByIdAsync(id);
                if (response != null)
                {
                    return new BaseResponse();
                }
                return new BaseResponse(ParkingAppConstants.GroupNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToDeleteGroup);
        }
    }
}
