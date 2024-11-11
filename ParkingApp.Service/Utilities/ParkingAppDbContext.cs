using Lisec.ParkingApp.Models;
using Lisec.UserManagementDB;
using Microsoft.EntityFrameworkCore;

namespace Lisec.ParkingApp.Utilities
{
    /// <summary>
    /// ParkingAppDbContext
    /// </summary>
    public class ParkingAppDbContext : UserManagementDBContext
    {
        /// <summary>
        /// DbSet for Card
        /// </summary>
        public DbSet<Card> Cards { get; set; }

        /// <summary>
        /// DbSet for UserCars
        /// </summary>
        public DbSet<UserCar> UserCars { get; set; }

        /// <summary>
        /// DbSet for Restrictions
        /// </summary>
        public DbSet<Restriction> Restrictions { get; set; }

        /// <summary>
        /// DbSet for Messages
        /// </summary>
        public DbSet<MessageModel> Messages { get; set; }

        /// <summary>
        /// DbSet for UserCards
        /// </summary>
        public DbSet<UserCard> UserCards { get; set; }

        /// <summary>
        /// DbSet for PaidParkings
        /// </summary>
        public DbSet<PaidParking> PaidParkings { get; set; }

        /// <summary>
        /// DbSet for Groups
        /// </summary>
        public DbSet<Group> Groups { get; set; }
    }
}
