using Lisec.ParkingApp.Utilities;
using Lisec.ServiceBase.QueryFilter;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Group = Lisec.ParkingApp.Models.Group;

namespace Lisec.ParkingApp.Repositories
{
    /// <summary>
    /// GroupsRepository
    /// </summary>
    public class GroupsRepository
    {
        private readonly ParkingAppDbContext _context;

        /// <summary>
        /// Class consructor
        /// </summary>
        /// <param name="context">Specify context</param>
        public GroupsRepository(ParkingAppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// To get all groups from database
        /// <returns>List of groups</returns>
        public async Task<IList<Group>> GetAllGroupsAsync(Query query)
        {
            return await _context.Groups.ApplyQuery(query).ToListAsync();
        }

        /// <summary>
        /// To get group by given id
        /// </summary>
        /// <param name="id">Specify group id</param>
        /// <returns>Group with given id</returns>
        public async Task<Group> GetGroupByIdAsync(int id)
        {
            return await _context.Groups.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// To add group in database
        /// </summary>
        /// <param name="groupToAdd">Specify groupToAdd</param>
        /// <returns>Group</returns>
        public async Task<Group> AddGroupAsync(Group groupToAdd)
        {
            await _context.Groups.AddAsync(groupToAdd);
            _context.UpdateModifiedPropertyInChangedEntries();
            await _context.SaveChangesAsync();
            return groupToAdd;
        }

        /// <summary>
        /// To update the existing group in database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="groupToUpdate">Specify groupToUpdate</param>
        /// <returns>Group or null</returns>
        public async Task<Group> UpdateGroupAsync(int id, Group groupToUpdate)
        {
            if (await _context.Groups.AnyAsync(x => x.Id == id))
            {
                groupToUpdate.Id = id;
                _context.Groups.Update(groupToUpdate);
                _context.UpdateModifiedPropertyInChangedEntries();
                await _context.SaveChangesAsync();
                return groupToUpdate;
            }
            return null;
        }

        /// <summary>
        /// To delete the group from database
        /// </summary>
        /// <param name="id">Specify id of group</param>
        /// <returns>Group or null</returns>
        public async Task<Group> DeleteGroupByIdAsync(int id)
        {

            Group groupEntity = await _context.Groups.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
            if (groupEntity == null)
                return null;
            groupEntity.IsDeleted = true;
            _context.Groups.Update(groupEntity);
            _context.UpdateModifiedPropertyInChangedEntries();
            await _context.SaveChangesAsync();
            return groupEntity;
        }
    }
}
