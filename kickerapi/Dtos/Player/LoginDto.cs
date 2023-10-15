

using System.ComponentModel.DataAnnotations;

namespace kickerapi.Dtos.Player
{
    public class LoginDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
