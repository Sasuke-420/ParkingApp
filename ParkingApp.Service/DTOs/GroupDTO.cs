using NJsonSchema.Annotations;
using System;
using System.Collections.Generic;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// GroupDTO
    /// </summary>
    [JsonSchema("Group")]
    public class GroupDTO
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Comma seperated ids of users
        /// </summary>
        public List<int> MemeberIds { get; set; }

        /// <summary>
        /// List of users
        /// </summary>
        public List<UserDTO> Users { get; set; }

        /// <summary>
        /// Name of group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Modified
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Is deleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
