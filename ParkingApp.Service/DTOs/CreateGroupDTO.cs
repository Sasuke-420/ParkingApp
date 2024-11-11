using NJsonSchema.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// CreateGroupDTO
    /// </summary>
    [JsonSchema("CreateGroup")]
    public class CreateGroupDTO
    {
        /// <summary>
        /// Comma seperated ids of users
        /// </summary>
        [Required]
        public List<int> MemeberIds { get; set; }

        /// <summary>
        /// Name of group
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Modified
        /// </summary>
        public DateTime Modified { get; set; }
    }
}
