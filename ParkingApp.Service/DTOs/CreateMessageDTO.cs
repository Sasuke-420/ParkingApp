using NJsonSchema.Annotations;
using System;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// CreateMessageDTO
    /// </summary>
    [JsonSchema("CreateMessage")]
    public class CreateMessageDTO
    {
        /// <summary>
        /// UserId
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Modified
        /// </summary>
        public DateTime Modified { get; set; }
    }
}
