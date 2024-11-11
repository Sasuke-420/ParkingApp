using Lisec.Base.Utilities.ResponseUtilities;
using Lisec.ParkingApp.Models;
using Lisec.UserManagementDB.Domain.Models.Master;
using Lisec.UserManagementDB.Domain.Utility;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Utilities
{
    /// <summary>
    /// ParkingAppUtility
    /// </summary>
    public class ParkingAppUtility
    {
        private readonly ParkingAppDbContext _context;
        private readonly ADUserUtility _adUserUtil;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="context">Specify ParkingAppDbContext</param>
        /// <param name="utility">Specify ADUserUtility</param>
        public ParkingAppUtility(ParkingAppDbContext context, ADUserUtility utility)
        {
            _context = context;
            _adUserUtil = utility;
        }

        /// <summary>
        /// To check if card is currently assigned to some one or not
        /// </summary>
        /// <param name="id">Specify id of card</param>
        /// <returns></returns>
        public async Task<bool> CheckCardInUseCurrentlyAsync(int id)
        {
            return await _context.UserCards.AnyAsync(x => x.CardId == id && DateOnly.FromDateTime(x.Date.ToUniversalTime().Date) == DateOnly.FromDateTime(DateTime.Today.ToUniversalTime().Date) && !x.IsDeleted);
        }

        /// <summary>
        /// To check if user with given id exists or not
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> CheckUserIdExistsAsync(int userId)
        {
            return await _context.MasterUser.AnyAsync(x => x.UserId == userId);
        }

        /// <summary>
        /// To check card with given id exists or not
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public async Task<bool> CheckCardIdExistsAsync(int cardId)
        {
            return await _context.Cards.AnyAsync(x => x.Id == cardId);
        }

        /// <summary>
        /// To check if weeks of two dates are equal or not
        /// </summary>
        /// <param name="date1">Specify date1</param>
        /// <param name="date2">Specify date2</param>
        /// <returns></returns>
        public bool AreWeeksEqual(DateTime date1, DateTime date2)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            Calendar calendar = culture.Calendar;

            int week1 = calendar.GetWeekOfYear(date1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            int week2 = calendar.GetWeekOfYear(date2, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            return week1 == week2;
        }

        /// <summary>
        /// To get all cards which are active
        /// </summary>
        /// <returns>List of cards</returns>
        public async Task<List<Card>> GetAllAvailableCardsAsync()
        {
            var usedCards = (from c in _context.Cards
                             join uc in _context.UserCards on c.Id equals uc.CardId
                             where uc.IsDeleted == false
                             select c).ToList();
            var totalCards = await _context.Cards.Where(x => !x.IsDeleted).ToListAsync();
            return totalCards.Except(usedCards).ToList();
        }

        /// <summary>
        /// To get all cards on specific date which are available
        /// </summary>
        /// <param name="date">Specify date</param>
        /// <returns>List of cards</returns>
        public async Task<List<Card>> GetAvailableCardsAsync(DateTime date)
        {
            var now = DateTime.UtcNow;
            var usedCards = (from c in _context.Cards
                             join uc in _context.UserCards on c.Id equals uc.CardId
                             where uc.IsDeleted == false && uc.Date.Date == date.Date && c.ExpiresOn > now
                             select c).ToList();
            var totalCards = await _context.Cards.Where(x => !x.IsDeleted).ToListAsync();
            return totalCards.Except(usedCards).ToList();
        }

        /// <summary>
        /// TO check if card is expired or not
        /// </summary>
        /// <param name="cardId">Specify card id</param>
        /// <returns></returns>
        public async Task<bool> CheckCardExpiredAsync(int cardId)
        {
            return (await _context.Cards.FirstOrDefaultAsync(x => x.Id == cardId)).ExpiresOn <= DateTime.UtcNow;
        }

        /// <summary>
        /// Extract claimValue based on the claim type
        /// </summary>
        /// <param name="user">ClaimsPrincipal</param>
        /// <param name="claimType">ClaimConstants.ClaimName, ClaimConstants.ClaimEmail ..</param>
        /// <returns>Returns null or valid claim value based on claim type</returns>
        public string GetClaimValue(ClaimsPrincipal user, string claimType)
        {
            Claim claim = user?.FindFirst(claimType);
            return claim?.Value;
        }

        /// <summary>
        /// To get car number by user id
        /// </summary>
        /// <param name="userId">Specify id of user</param>
        /// <returns>Car number</returns>
        public async Task<List<string>> GetCarNumberByUserIdAsync(int userId)
        {
            return await _context.UserCars.Where(x => x.UserId == userId).Select(x => x.CarNumber).ToListAsync();
        }

        /// <summary>
        /// To check if provided list of user id exists or not
        /// </summary>
        /// <param name="memeberIds">Specify memeberIds</param>
        /// <returns></returns>
        public bool HasValidUserIds(List<int> memeberIds)
        {
            return memeberIds.Any(y => !_context.MasterUser.Any(x => x.UserId == y));
        }

        /// <summary>
        /// To get list of users with given id
        /// </summary>
        /// <param name="memeberIds">Specify member list</param>
        /// <returns></returns>
        public List<MasterUser> GetUserList(List<int> memeberIds)
        {
            List<MasterUser> result = new List<MasterUser>();
            memeberIds.ForEach(x =>
            {
                var user = _context.MasterUser.FirstOrDefault(y => y.UserId == x);
                result.Add(user);
            });
            return result;
        }

        /// <summary>
        /// To check given group id is valid or not
        /// </summary>
        /// <param name="groupId">Specify groupId</param>
        /// <returns></returns>
        public bool ValidGroupId(int groupId)
        {
            return _context.Groups.Any(x => x.Id == groupId);
        }

        /// <summary>
        /// TO get users i by group
        /// </summary>
        /// <param name="id">Specify group id</param>
        /// <returns></returns>
        public async Task<List<int>> GetUsersIdByGroupAsync(int id)
        {
            return (await _context.Groups.FirstOrDefaultAsync(x => x.Id == id)).MemeberIds.Split(',').Select(x => int.Parse(x)).ToList();
        }

        /// <summary>
        /// To get user id from email
        /// </summary>
        /// <param name="emailId">Specify emailId</param>
        public async Task<int?> GetUserIdFromEmail(string emailId)
        {
            MasterUser user = await _context.MasterUser.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == emailId.ToLower());

            if (user == null)
            {
                BaseResponse<IList<MasterUser>> response = await _adUserUtil.CheckUserAvaliableInADAsync(emailId).ConfigureAwait(false);
                if (response.IsSuccessStatusCode())
                    return response.Resource.FirstOrDefault().UserId;
                return null;
            }
            return user.UserId;
        }

        /// <summary>
        /// To get restriction amount
        /// </summary>
        /// <returns></returns>
        public async Task<Restriction> GetRestrictionAsync()
        {
            return await _context.Restrictions.FirstOrDefaultAsync();
        }

        /// <summary>
        /// To get the user ids who have already assigned himself card on the specified day
        /// </summary>
        /// <param name="date">Specify date</param>
        /// <returns></returns>
        public async Task<List<int>> GetUserCardAsync(DateTime date)
        {
            return await _context.UserCards.Where(x => x.Date.Date == date.Date && !x.IsDeleted).Select(x => x.UserId).ToListAsync();
        }
    }
}
