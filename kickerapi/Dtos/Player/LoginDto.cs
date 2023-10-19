

using System.ComponentModel.DataAnnotations;

namespace kickerapi.Dtos.Player
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
