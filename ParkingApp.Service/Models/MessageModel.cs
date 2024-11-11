using Lisec.Base.Database.Entities;
using Lisec.UserManagementDB.Domain.Models.Master;
using System.ComponentModel.DataAnnotations;

namespace Lisec.ParkingApp.Models
{
    /// <summary>
    /// MessageModel
    /// </summary>
    public class MessageModel : BaseMasterEntity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// User
        /// </summary>
        public MasterUser User { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }
    }
}
