using NJsonSchema.Annotations;
using System;
using System.Collections.Generic;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// UserAttendanceDTO
    /// </summary>
    [JsonSchema("UserAttendance")]
    public class UserAttendanceDTO
    {
        /// <summary>
        /// UserId
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Days
        /// </summary>
        public List<DateTime> Days { get; set; }
    }
}
