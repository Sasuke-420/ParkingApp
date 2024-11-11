using Lisec.ParkingApp.Models;
using Lisec.ParkingApp.Utilities;
using Lisec.ServiceBase.QueryFilter;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class PaidParkingsRepository
    {
        private readonly ParkingAppDbContext _context;

        /// <summary>
        /// Class consructor
        /// </summary>
        /// <param name="context">Specify context</param>
        public PaidParkingsRepository(ParkingAppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// To get all paid parkings from database
        /// <returns>List of paid parkings</returns>
        public async Task<IList<PaidParking>> GetAllPaidParkingsAsync(Query query)
        {
            return await _context.PaidParkings.ApplyQuery(query).ToListAsync();
        }

        /// <summary>
        /// To get paid parking by given id
        /// </summary>
        /// <param name="id">Specify paid parking id</param>
        /// <returns>PaidParking with given id</returns>
        public async Task<PaidParking> GetPaidParkingByIdAsync(int id)
        {
            return await _context.PaidParkings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// To add paid parking in database
        /// </summary>
        /// <param name="paidParkingToAdd">Specify paid parkingToAdd</param>
        /// <returns>PaidParking</returns>
        public async Task<PaidParking> AddPaidParkingAsync(PaidParking paidParkingToAdd)
        {
            await _context.PaidParkings.AddAsync(paidParkingToAdd);
            _context.UpdateModifiedPropertyInChangedEntries();
            await _context.SaveChangesAsync();
            return paidParkingToAdd;
        }

        /// <summary>
        /// To update the existing paid parking in database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="paidParkingToUpdate">Specify paid parkingToUpdate</param>
        /// <returns>PaidParking or null</returns>
        public async Task<PaidParking> UpdatePaidParkingAsync(int id, PaidParking paidParkingToUpdate)
        {
            if (await _context.PaidParkings.AnyAsync(x => x.Id == id))
            {
                paidParkingToUpdate.Id = id;
                _context.PaidParkings.Update(paidParkingToUpdate);
                _context.UpdateModifiedPropertyInChangedEntries();
                await _context.SaveChangesAsync();
                return paidParkingToUpdate;
            }
            return null;
        }

        /// <summary>
        /// To delete the paid parking from database
        /// </summary>
        /// <param name="paidParkingEntity">Specify PaidParking</param>
        /// <returns>PaidParking or null</returns>
        public async Task<PaidParking> DeletePaidParkingAsync(PaidParking paidParkingEntity)
        {
            _context.PaidParkings.Remove(paidParkingEntity);
            await _context.SaveChangesAsync();
            return paidParkingEntity;
        }

        /// <summary>
        /// To delete the paid parking from database
        /// </summary>
        /// <param name="id">Specify id of paid parking</param>
        /// <returns>PaidParking or null</returns>
        public async Task<PaidParking> DeletePaidParkingByIdAsync(int id)
        {

            PaidParking paidParkingEntity = await _context.PaidParkings.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
            if (paidParkingEntity == null)
                return null;
            _context.PaidParkings.Remove(paidParkingEntity);
            await _context.SaveChangesAsync();
            return paidParkingEntity;
        }

        /// <summary>
        /// TO get paid parking records upto specific date
        /// </summary>
        /// <param name="lastParkingDate">Specify lastParkingDate</param>
        /// <returns></returns>
        public async Task<List<PaidParking>> GetPaidParkingsUptoDateAsync(DateTime lastParkingDate)
        {
            return await _context.PaidParkings.AsNoTracking().Where(x => x.Date.Date <= lastParkingDate.Date && !x.Settled).ToListAsync();
        }

        /// <summary>
        /// TO get paid parking of specific user records for specific date
        /// </summary>
        /// <param name="parkingDate">Specify parkingDate</param>
        /// <param name="userId">Specify userId</param>
        /// <returns></returns>
        public async Task<List<PaidParking>> GetPaidParkingsOfUserUptoDateAsync(DateTime parkingDate, int userId)
        {
            return await _context.PaidParkings.AsNoTracking().Where(x => x.Date.Date == parkingDate.Date && !x.Settled && x.UserId == userId).ToListAsync();
        }

        /// <summary>
        /// TO remove paid parking records in bulk
        /// </summary>
        /// <param name="paidParkingRecords">Specify paidParkingRecords</param>
        public void RemoveBulk(List<PaidParking> paidParkingRecords)
        {
            _context.PaidParkings.RemoveRange(paidParkingRecords);
            _context.SaveChanges();
        }

        /// <summary>
        /// To get the paid parking records where payer has share
        /// </summary>
        /// <param name="payeeId">Specify payeeId</param>
        /// <param name="payerId">Specify payerId</param>
        /// <returns></returns>
        public async Task<List<PaidParking>> GetBalanceAsync(int payeeId, int payerId)
        {
            List<PaidParking> result = new List<PaidParking>();
            await _context.PaidParkings.AsNoTracking().ForEachAsync(x =>
            {
                var payerIdExists = x.SharesId.Split(',').Select(x => int.Parse(x)).Contains(payerId);
                if (x.UserId == payeeId && payerIdExists)
                {
                    result.Add(x);
                }
            });
            return result;
        }

        /// <summary>
        /// To get all the records where user with given id is debtor or creditor
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns></returns>
        public async Task<List<PaidParking>> GetDuesAsync(int id)
        {
            List<PaidParking> result = new List<PaidParking>();
            await _context.PaidParkings.AsNoTracking().ForEachAsync(x =>
            {
                var payerIdExists = x.SharesId.Split(',').Select(x => int.Parse(x)).Contains(id);
                if (x.UserId == id || payerIdExists)
                {
                    result.Add(x);
                }
            });
            return result;
        }
    }
}
