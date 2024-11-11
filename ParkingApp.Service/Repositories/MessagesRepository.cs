using Lisec.ParkingApp.Models;
using Lisec.ParkingApp.Utilities;
using Lisec.ServiceBase.QueryFilter;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lisec.ParkingApp.Repositories
{
    /// <summary>
    /// MessagesRepository
    /// </summary>
    public class MessagesRepository
    {
        private readonly ParkingAppDbContext _context;

        /// <summary>
        /// Class consructor
        /// </summary>
        /// <param name="context">Specify context</param>
        public MessagesRepository(ParkingAppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// To get all messages from database
        /// <returns>List of messages</returns>
        public async Task<IList<MessageModel>> GetAllMessagesAsync(Query query)
        {
            return await _context.Messages.ApplyQuery(query).ToListAsync();
        }

        /// <summary>
        /// To get message by given id
        /// </summary>
        /// <param name="id">Specify message id</param>
        /// <returns>Message with given id</returns>
        public async Task<MessageModel> GetMessageByIdAsync(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// To add message in database
        /// </summary>
        /// <param name="messageToAdd">Specify messageToAdd</param>
        /// <returns>Message</returns>
        public async Task<MessageModel> AddMessageAsync(MessageModel messageToAdd)
        {
            await _context.Messages.AddAsync(messageToAdd);
            _context.UpdateModifiedPropertyInChangedEntries();
            await _context.SaveChangesAsync();
            return messageToAdd;
        }

        /// <summary>
        /// To update the existing message in database
        /// </summary>
        /// <param name="id">Specify id</param>
        /// <param name="messageToUpdate">Specify messageToUpdate</param>
        /// <returns>Message or null</returns>
        public async Task<MessageModel> UpdateMessageAsync(int id, MessageModel messageToUpdate)
        {
            if (await _context.Messages.AnyAsync(x => x.Id == id))
            {
                messageToUpdate.Id = id;
                _context.Messages.Update(messageToUpdate);
                _context.UpdateModifiedPropertyInChangedEntries();
                await _context.SaveChangesAsync();
                return messageToUpdate;
            }
            return null;
        }

        /// <summary>
        /// To delete the message from database
        /// </summary>
        /// <param name="id">Specify id of message</param>
        /// <returns>Message or null</returns>
        public async Task<MessageModel> DeleteMessageByIdAsync(int id)
        {
            MessageModel messageEntity = await _context.Messages.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);
            if (messageEntity == null)
                return null;
            _context.Messages.Remove(messageEntity);
            await _context.SaveChangesAsync();
            return messageEntity;
        }
    }
}
