using NJsonSchema.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// CreateUserCardDTO
    /// </summary>
    [JsonSchema("CreateUserCard")]
    public class CreateUserCardDTO
    {
        /// <summary>
        /// UserId
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// CardId
        /// </summary>
        [Required]
        public int CardId { get; set; }

        /// <summary>
        /// Date
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

        /// <summary>
        /// Modified
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
