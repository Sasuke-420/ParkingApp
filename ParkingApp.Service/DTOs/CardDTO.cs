using NJsonSchema.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// CardDTO
    /// </summary>
    [JsonSchema("Card")]
    public class CardDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// CardNumber
        /// </summary>
        [Required]
        public string CardNumber { get; set; }

        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Modified
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// ExpiresOn
        /// </summary>
        public DateTime ExpiresOn { get; set; }
    }
}
