using NJsonSchema.Annotations;

namespace Lisec.ParkingApp.DTOs
{
    /// <summary>
    /// UserDTO
    /// </summary>
    [JsonSchema("User")]
    public class UserDTO
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
    }
}
