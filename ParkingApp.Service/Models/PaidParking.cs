using Lisec.Base.Database.Entities;
using Lisec.UserManagementDB.Domain.Models.Master;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lisec.ParkingApp.Models
{
    /// <summary>
    /// PaidParking
    /// </summary>
    public class PaidParking : BaseMasterEntity
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Id of user who paid the amount
        /// </summary>
        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }

        /// <summary>
        /// User object
        /// </summary>
        public MasterUser User { get; set; }

        /// <summary>
        /// Amount paid
        /// </summary>
        [Required]
        public double AmountPaid { get; set; }

        // TODO can use to keep history of payments
        /// <summary>
        /// Settled
        /// </summary>
        public bool Settled { get; set; } = false;

        /// <summary>
        /// Date at which amount was paid
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// List of users as string between amount will be shared
        /// </summary>
        public string SharesId { get; set; }
    }
}
