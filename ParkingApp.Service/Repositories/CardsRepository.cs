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
    /// CardsRepository
    /// </summary>
    public class CardsRepository
    {
        private readonly ParkingAppDbContext _context;

        /// <summary>
        /// Class consructor
        /// </summary>
        /// <param name="context">Specify context</param>
        public CardsRepository(ParkingAppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// To get all cards from database
        /// <returns>List of cards</returns>
        public async Task<IList<Card>> GetAllCardsAsync(Query query)
        {
            return await _context.Cards.ApplyQuery(query).ToListAsync();
        }

        /// <summary>
        /// To get card by given id
        /// </summary>
        /// <param name="id">Specify card id</param>
        /// <returns>Card with given id</returns>
        public async Task<Card> GetCardByIdAsync(int id)
        {
            return await _context.Cards.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// To add card in database
        /// </summary>
        /// <param name="cardToAdd">Specify cardToAdd</param>
        /// <returns>Card</returns>
        public async Task<Card> AddCardAsync(Card cardToAdd)
        {
            await _context.Cards.AddAsync(cardToAdd);
            _context.UpdateModifiedPropertyInChangedEntries();
            await _context.SaveChangesAsync();
            return cardToAdd;
        }

        /// <summary>
        /// To update the existing card in database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="cardToUpdate">Specify cardToUpdate</param>
        /// <returns>Card or null</returns>
        public async Task<Card> UpdateCardAsync(int id, Card cardToUpdate)
        {
            if (await _context.Cards.AnyAsync(x => x.Id == id))
            {
                cardToUpdate.Id = id;
                _context.Cards.Update(cardToUpdate);
                _context.UpdateModifiedPropertyInChangedEntries();
                await _context.SaveChangesAsync();
                return cardToUpdate;
            }
            return null;
        }

        /// <summary>
        /// To delete the card from database
        /// </summary>
        /// <param name="id">Specify id of card</param>
        /// <returns>Card or null</returns>
        public async Task<Card> DeleteCardByIdAsync(int id)
        {
            Card cardEntity = await _context.Cards.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
            if (cardEntity == null)
                return null;
            cardEntity.IsDeleted = true;
            _context.Cards.Update(cardEntity);
            _context.UpdateModifiedPropertyInChangedEntries();
            await _context.SaveChangesAsync();
            return cardEntity;
        }

        /// <summary>
        /// To get all the available cards
        /// </summary>
        /// <param name="date">Specify date</param>
        /// <returns></returns>
        public async Task<List<Card>> GetAvailableCardsAsync(DateTime date)
        {
            var now = DateTime.UtcNow;
            return await _context.Cards.Where(x => !_context.UserCards.Any(y => y.CardId == x.Id && y.Date.Date == date && !y.IsDeleted) && x.ExpiresOn <= now.Date && !x.IsDeleted).ToListAsync();
        }

        /// <summary>
        /// To check if card with same no already exists
        /// </summary>
        /// <param name="cardNumber">Specify cardNumber</param>
        /// <param name="isUpdate">Specify isUpdate</param>
        /// <param name="id">Specify id if update</param>
        /// <returns></returns>
        public async Task<bool> CheckCardAlreadyExistsAsync(string cardNumber, bool isUpdate = false, int? id = null)
        {
            if (!isUpdate)
            {
                return await _context.Cards.AnyAsync(x => x.CardNumber.ToLower() == cardNumber.ToLower());
            }
            return await _context.Cards.AnyAsync(x => x.Id != id && x.CardNumber.ToLower() == cardNumber.ToLower());
        }
    }
}
