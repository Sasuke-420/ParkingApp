using NJsonSchema.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// UserCarDTO
    /// </summary>
    [JsonSchema("UserCar")]
    public class UserCarDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// User
        /// </summary>
        public UserDTO User { get; set; }

        /// <summary>
        /// CarNumber
        /// </summary>
        public string CarNumber { get; set; }

        /// <summary>
        /// Modified
        /// </summary>
        public DateTime Modified { get; set; }
    }
}
