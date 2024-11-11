using NJsonSchema.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// CreateUserCarDTO
    /// </summary>
    [JsonSchema("CreateUserCar")]
    public class CreateUserCarDTO
    {
        /// <summary>
        /// UserId
        /// </summary>
        [Required]
        public int UserId { get; set; }

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
