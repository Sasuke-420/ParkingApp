using System;

namespace Lisec.ParkingApp.DTOs
{
    public class RestrictionDTO : UpsertRestrictionDTO
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Modified
        /// </summary>
        public DateTime Modified { get; set; }
    }
}
