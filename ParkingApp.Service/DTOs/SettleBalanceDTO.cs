using NJsonSchema.Annotations;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// SettleBalanceDTO
    /// </summary>
    [JsonSchema("SettleBalance")]
    public class SettleBalanceDTO
    {
        /// <summary>
        /// User id who is paying amount to settle balance
        /// </summary>
        public int PayerId { get; set; }

        /// <summary>
        /// Amount 
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        /// User id of receiver
        /// </summary>
        public int PayeeId { get; set; }
    }
}
