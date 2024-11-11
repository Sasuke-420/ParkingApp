using NJsonSchema.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// CreatePaidParkingDTO
    /// </summary>
    [JsonSchema("CreatePaidParking")]
    public class CreatePaidParkingDTO
    {
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
        /// List of users as string between amount will be shared
        /// </summary>
        public List<int> SharesId { get; set; }

        /// <summary>
        /// If GroupId is sent instead of shares id then get user ids from group table
        /// </summary>
        public int? GroupId { get; set; }

        /// <summary>
        /// Modified
        /// </summary>
        public DateTime Modified { get; set; }
    }
}
