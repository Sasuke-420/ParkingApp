using Lisec.Base.Database.Entities;
using System.ComponentModel.DataAnnotations;

namespace Lisec.ParkingApp.Models
{
    /// <summary>
    /// Restriction
    /// </summary>
    public class Restriction : BaseMasterEntity
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Amount upto which need to restrict
        /// </summary>
        public double Amount { get; set; }
    }
}
