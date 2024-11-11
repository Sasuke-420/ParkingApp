using Lisec.Base.Database.Entities;
using Lisec.UserManagementDB.Domain.Models.Master;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lisec.ParkingApp.Models
{
    /// <summary>
    /// UserCard
    /// </summary>
    public class UserCard : BaseMasterEntity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        /// <summary>
        /// User object
        /// </summary>
        public MasterUser User { get; set; }

        /// <summary>
        /// CardId
        /// </summary>
        [Required]
        [ForeignKey(nameof(Card))]
        public int CardId { get; set; }

        /// <summary>
        /// Object of card
        /// </summary>
        public Card Card { get; set; }

        /// <summary>
        /// Deleted or not
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Date of usage
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public string Longitude { get; set; }
    }
}
