using NJsonSchema.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// CreateCardDTO
    /// </summary>
    [JsonSchema("CreateCard")]
    public class CreateCardDTO
    {
        /// <summary>
        /// CardNumber
        /// </summary>
        [Required]
        public string CardNumber { get; set; }

        /// <summary>
        /// Modified
        /// </summary>
        public DateTime Modified { get; set; }

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
