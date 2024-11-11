using NJsonSchema.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// PaidParkingDTO
    /// </summary>
    [JsonSchema("PaidParking")]
    public class PaidParkingDTO
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
        public int UserId { get; set; }

        /// <summary>
        /// Amount paid
        /// </summary>
        [Required]
        public double AmountPaid { get; set; }

        /// <summary>
        /// Date at which amount was paid
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// List of users
        /// </summary>
        public List<UserDTO> Users { get; set; }

        /// <summary>
        /// Modified
        /// </summary>
        public DateTime Modified { get; set; }
    }
}
