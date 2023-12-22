

using System.ComponentModel.DataAnnotations;

namespace kickerapi.Dtos.Requests.Security
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }
}
