using System.ComponentModel.DataAnnotations;

namespace NetworkServicesGateway.Models
{
    public class User
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
}
