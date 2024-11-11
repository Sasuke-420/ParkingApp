using Lisec.ParkingApp.Models;
using Lisec.ParkingApp.Utilities;
using Lisec.ServiceBase.QueryFilter;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Repositories
{
    /// <summary>
    /// UserCardsRepository
    /// </summary>
    public class UserCardsRepository
    {
        private readonly ParkingAppDbContext _context;

        /// <summary>
        /// Class consructor
        /// </summary>
        /// <param name="context">Specify context</param>
        public UserCardsRepository(ParkingAppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// To get all user card from database
        /// </summary>
        /// <param name="query">Specify RSQL query</param>
        /// <returns>List of user cards</returns>
        public async Task<IList<UserCard>> GetAllUserCardsAsync(Query query)
        {
            return await _context.UserCards.Include(x => x.Card).Include(x => x.User).ApplyQuery(query).ToListAsync();
        }

        /// <summary>
        /// To get user card by id
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns>UserCard</returns>
        public async Task<UserCard> GetUserCardByIdAsync(int id)
        {
            return await _context.UserCards.Include(x => x.Card).Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// To add new user card record to database
        /// </summary>
        /// <param name="userCardToAdd">Specify userCardToAdd</param>
        /// <returns>UserCard</returns>
        public async Task<UserCard> AddUserCardAsync(UserCard userCardToAdd)
        {
            await _context.UserCards.AddAsync(userCardToAdd);
            _context.UpdateModifiedPropertyInChangedEntries();
            await _context.SaveChangesAsync();
            return userCardToAdd;
        }

        /// <summary>
        /// To update existing user card record
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="userCardToUpdate">Specify userCardToUpdate</param>
        /// <returns>UserCard or null</returns>
        public async Task<UserCard> UpdateUserCardAsync(int id, UserCard userCardToUpdate)
        {
            if (await _context.UserCards.AnyAsync(x => x.Id == id))
            {
                userCardToUpdate.Id = id;
                _context.UserCards.Update(userCardToUpdate);
                _context.UpdateModifiedPropertyInChangedEntries();
                await _context.SaveChangesAsync();
                return userCardToUpdate;
            }
            return null;
        }

        /// <summary>
        /// To delete existing user card record from database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <returns>UserCard or null</returns>
        public async Task<UserCard> DeleteUserCardByIdAsync(UserCard UserCardEntity)
        {
            UserCardEntity.IsDeleted = true;
            _context.UserCards.Update(UserCardEntity);
            _context.UpdateModifiedPropertyInChangedEntries();
            await _context.SaveChangesAsync();
            return UserCardEntity;
        }

        /// <summary>
        /// To check if user has already assigned itself card
        /// </summary>
        /// <param name="userId">Specify userId</param>
        /// <returns></returns>
        public async Task<UserCard> GetCardUserAsync(int userId)
        {
            return await _context.UserCards.FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted);
        }

        /// <summary>
        /// To add user cards record in bulk
        /// </summary>
        /// <param name="userCards">Specify user cards list</param>
        /// <returns></returns>
        public async Task<List<UserCard>> AddUserCardsInBulkAsync(List<UserCard> userCards)
        {
            foreach (var userCard in userCards)
            {
                await _context.UserCards.AddAsync(userCard);
                _context.UpdateModifiedPropertyInChangedEntries();
            }
            await _context.SaveChangesAsync();
            return userCards;
        }
    }
}
