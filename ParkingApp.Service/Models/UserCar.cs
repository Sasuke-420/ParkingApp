using Lisec.Base.Database.Entities;
using Lisec.UserManagementDB.Domain.Models.Master;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;

namespace Lisec.ParkingApp.Models
{
    /// <summary>
    /// UserCar
    /// </summary>
    [Index(nameof(CarNumber), IsUnique = true)]
    public class UserCar : BaseMasterEntity
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
        public virtual MasterUser User { get; set; }

        /// <summary>
        /// Car Number (must be unique)
        /// </summary>
        public string CarNumber { get; set; }
    }
}
