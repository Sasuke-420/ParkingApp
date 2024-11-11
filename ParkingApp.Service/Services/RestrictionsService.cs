using AutoMapper;
using Lisec.Base.Utilities.ResponseUtilities;
using Lisec.ParkingApp.DTOs;
using Lisec.ParkingApp.Models;
using Lisec.ParkingApp.Repositories;
using Lisec.ParkingApp.Utilities;
using Lisec.ServiceBase.Utilities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Services
{
    public class RestrictionsService
    {
        private readonly RestrictionsRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Class constructor
        /// </summary>
        public RestrictionsService(RestrictionsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// To get all restrictions from database
        /// </summary>
        /// <param name="query">Specifty query</param>
        /// <returns>List of restrictions</returns>
        public async Task<BaseResponse<IList<RestrictionDTO>>> GetAllRestrictionsAsync()
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync<IList<RestrictionDTO>>(async () =>
            {
                IList<Restriction> response = await _repository.GetAllRestrictionsAsync();
                if (response.Count > 0)
                {
                    IList<RestrictionDTO> mappedResponse = _mapper.Map<IList<Restriction>, IList<RestrictionDTO>>(response);
                    return new BaseResponse<IList<RestrictionDTO>>(mappedResponse);
                }
                return new BaseResponse<IList<RestrictionDTO>>(ParkingAppConstants.NoContentAvailable, StatusCodes.Status204NoContent);
            }, ParkingAppConstants.FailedToGetRestriction);
        }

        /// <summary>
        /// To add new restriction to database
        /// </summary>
        /// <param name="createRestrictionRequest">Specify createRestrictionRequest</param>
        /// <returns>CreateRestrictionDTO</returns>
        public async Task<BaseResponse<RestrictionDTO>> AddRestrictionAsync(UpsertRestrictionDTO createRestrictionRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (createRestrictionRequest == null)
                    return new BaseResponse<RestrictionDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);

                var restrictionToAdd = _mapper.Map<UpsertRestrictionDTO, Restriction>(createRestrictionRequest);
                var existingRecord = await _repository.GetAllRestrictionsAsync();
                if (existingRecord.Count > 0)
                {
                    return new BaseResponse<RestrictionDTO>(ParkingAppConstants.CannotAddRestriction, StatusCodes.Status412PreconditionFailed);
                }
                Restriction response = await _repository.AddRestrictionAsync(restrictionToAdd);

                RestrictionDTO mappedResponse = _mapper.Map<Restriction, RestrictionDTO>(response);
                return new BaseResponse<RestrictionDTO>(mappedResponse);

            }, ParkingAppConstants.FailedToAddRestriction);
        }

        /// <summary>
        /// To update existing restriction in database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="upsertRestrictionRequest">Specify upsertRestrictionRequest</param>
        /// <returns>CreateRestrictionDTO</returns>
        public async Task<BaseResponse<RestrictionDTO>> UpsertRestrictionAsync(UpsertRestrictionDTO upsertRestrictionRequest)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                if (upsertRestrictionRequest == null)
                    return new BaseResponse<RestrictionDTO>(ParkingAppConstants.RequestNotParsable, StatusCodes.Status400BadRequest);

                var restrictionToUpdate = _mapper.Map<UpsertRestrictionDTO, Restriction>(upsertRestrictionRequest);
                Restriction response = await _repository.UpsertRestrictionAsync(restrictionToUpdate);
                if (response != null)
                {
                    RestrictionDTO mappedResponse = _mapper.Map<Restriction, RestrictionDTO>(response);
                    return new BaseResponse<RestrictionDTO>(mappedResponse);
                }
                return new BaseResponse<RestrictionDTO>(ParkingAppConstants.RestrictionNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToUpdateRestriction);
        }

        // TODO delete all the records
        /// <summary>
        /// To delete the existing restriction from database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns></returns>
        public async Task<BaseResponse> DeleteRestrictionByIdAsync(int id)
        {
            return await ServiceBaseUtility.RunFuncWithConcurrencyCheckAsync(async () =>
            {
                var response = await _repository.DeleteRestrictionByIdAsync(id);
                if (response != null)
                {
                    return new BaseResponse();
                }
                return new BaseResponse(ParkingAppConstants.RestrictionNotFound, StatusCodes.Status404NotFound);
            }, ParkingAppConstants.FailedToDeleteRestriction);
        }
    }
}
