using NJsonSchema.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// UserCardDTO
    /// </summary>
    [JsonSchema("UserCard")]
    public class UserCardDTO
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
        /// CardId
        /// </summary>
        [Required]
        public int CardId { get; set; }

        /// <summary>
        /// Card
        /// </summary>
        public CardDTO Card { get; set; }

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
        /// CarNumber
        /// </summary>
        public List<string> CarNumber { get; set; }

        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
