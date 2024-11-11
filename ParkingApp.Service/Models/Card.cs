using Lisec.Base.Database.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lisec.ParkingApp.Models
{
    /// <summary>
    /// Card
    /// </summary>
    public class Card : BaseMasterEntity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Card no.
        /// </summary>
        [Required]
        public string CardNumber { get; set; }

        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// ExpiresOn
        /// </summary>
        public DateTime ExpiresOn { get; set; }
    }
}
