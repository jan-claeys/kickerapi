using System.ComponentModel.DataAnnotations;

namespace kickerapi.Dtos.Requests.Security
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
        [Required(ErrorMessage = "Email is required"), EmailAddress]
        public string Email { get; set; } = null!;
    }
}
