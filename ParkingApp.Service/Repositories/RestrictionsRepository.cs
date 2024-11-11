using Lisec.ParkingApp.Models;
using Lisec.ParkingApp.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Repositories
{
    /// <summary>
    /// RestrictionsRepository
    /// </summary>
    public class RestrictionsRepository
    {
        private readonly ParkingAppDbContext _context;

        /// <summary>
        /// Class consructor
        /// </summary>
        /// <param name="context">Specify context</param>
        public RestrictionsRepository(ParkingAppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// To get all restrictions from database
        /// <returns>List of restrictions</returns>
        public async Task<IList<Restriction>> GetAllRestrictionsAsync()
        {
            return await _context.Restrictions.ToListAsync();
        }

        /// <summary>
        /// To get restriction by given id
        /// </summary>
        /// <param name="id">Specify restriction id</param>
        /// <returns>Restriction with given id</returns>
        public async Task<Restriction> GetRestrictionByIdAsync(int id)
        {
            return await _context.Restrictions.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// To add restriction in database
        /// </summary>
        /// <param name="restrictionToAdd">Specify restrictionToAdd</param>
        /// <returns>Restriction</returns>
        public async Task<Restriction> AddRestrictionAsync(Restriction restrictionToAdd)
        {
            await _context.Restrictions.AddAsync(restrictionToAdd);
            _context.UpdateModifiedPropertyInChangedEntries();
            await _context.SaveChangesAsync();
            return restrictionToAdd;
        }

        /// <summary>
        /// To update the existing restriction in database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="restrictionToUpsert">Specify restrictionToUpsert</param>
        /// <returns>Restriction or null</returns>
        public async Task<Restriction> UpsertRestrictionAsync(Restriction restrictionToUpsert)
        {
            var existing = await _context.Restrictions.AsNoTracking().FirstOrDefaultAsync();
            if (existing != null)
            {
                existing.Amount = restrictionToUpsert.Amount;
                _context.Restrictions.Update(existing);
                restrictionToUpsert.Id = existing.Id;
                restrictionToUpsert.Modified = existing.Modified;
            }
            else
            {
                await _context.Restrictions.AddAsync(restrictionToUpsert);
            }
            _context.UpdateModifiedPropertyInChangedEntries();
            await _context.SaveChangesAsync();
            return restrictionToUpsert;
        }

        /// <summary>
        /// To delete the restriction from database
        /// </summary>
        /// <param name="id">Specify id of restriction</param>
        /// <returns>Restriction or null</returns>
        public async Task<Restriction> DeleteRestrictionByIdAsync(int id)
        {

            Restriction restrictionEntity = await _context.Restrictions.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
            if (restrictionEntity == null)
                return null;
            _context.Restrictions.Remove(restrictionEntity);
            await _context.SaveChangesAsync();
            return restrictionEntity;
        }
    }
}
