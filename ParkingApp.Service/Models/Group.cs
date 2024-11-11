using Lisec.Base.Database.Entities;

namespace Lisec.ParkingApp.Models
{
    /// <summary>
    /// Group
    /// </summary>
    public class Group : BaseMasterEntity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Comma seperated ids of users
        /// </summary>
        public string MemeberIds { get; set; }

        /// <summary>
        /// Name of group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
