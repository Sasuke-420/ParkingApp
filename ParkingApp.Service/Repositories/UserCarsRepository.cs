using Lisec.ParkingApp.Models;
using Lisec.ParkingApp.Utilities;
using Lisec.ServiceBase.QueryFilter;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Repositories
{
    /// <summary>
    /// UserCarsRepository
    /// </summary>
    public class UserCarsRepository
    {
        private readonly ParkingAppDbContext _context;

        /// <summary>
        /// Class consructor
        /// </summary>
        /// <param name="context">Specify context</param>
        public UserCarsRepository(ParkingAppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// To get list of user cars from database.
        /// </summary>
        /// <param name="query">Specify query</param>
        /// <returns>List of UserCar</returns>
        public async Task<IList<UserCar>> GetAllUserCarsAsync(Query query)
        {
            return await _context.UserCars.Include(x => x.User).ApplyQuery(query).ToListAsync();
        }

        /// <summary>
        /// To get user car by id
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns>Specify UserCar</returns>
        public async Task<UserCar> GetUserCarByIdAsync(int id)
        {
            return await _context.UserCars.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// To add new user car record to database
        /// </summary>
        /// <param name="cardToAdd">Specify cardToAdd</param>
        /// <returns>UserCar</returns>
        public async Task<UserCar> AddUserCarAsync(UserCar UsercarToAdd)
        {
            await _context.UserCars.AddAsync(UsercarToAdd);
            _context.UpdateModifiedPropertyInChangedEntries();
            await _context.SaveChangesAsync();
            return UsercarToAdd;
        }

        /// To update existing user car record
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="cardToUpdate">Specify cardToUpdate</param>
        /// <returns>UserCar or null</returns>
        public async Task<UserCar> UpdateUserCarAsync(int id, UserCar UserCarToUpdate)
        {
            /// <summary>
            try
            {
                //  var existing = await _context.UserCars.FirstOrDefaultAsync(x => x.Id == id);
                if (await _context.UserCars.AnyAsync(a => a.Id == id))
                {
                    UserCarToUpdate.Id = id;
                    _context.UserCars.Update(UserCarToUpdate);
                    _context.UpdateModifiedPropertyInChangedEntries();
                    await _context.SaveChangesAsync();
                    return UserCarToUpdate;
                }
                return null;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return null;
            }
        }

        /// <summary>
        /// To delete user car record by given id
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns>UserCar or null</returns>
        public async Task<UserCar> DeleteUserCarByIdAsync(int id)
        {
            UserCar UserCarEntity = await _context.UserCars.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
            if (UserCarEntity == null)
                return null;
            _context.UserCars.Remove(UserCarEntity);
            _context.UpdateModifiedPropertyInChangedEntries();
            await _context.SaveChangesAsync();
            return UserCarEntity;
        }

        /// <summary>
        /// To check if car is already registered or not
        /// </summary>
        /// <param name="carNumber">Specify carNumber</param>
        /// <param name="isUpdate">Specify isUpdate</param>
        /// <param name="id">Specify id</param>
        /// <returns></returns>
        public async Task<bool> CheckCarAlreadyRegistered(string carNumber, bool isUpdate = false, int? id = null)
        {
            if (!isUpdate)
                return await _context.UserCars.AnyAsync(a => a.CarNumber.ToLower() == carNumber.ToLower());

            return await _context.UserCars.AnyAsync(x => x.Id != id && x.CarNumber.ToLower() == carNumber.ToLower());
        }
    }
}
