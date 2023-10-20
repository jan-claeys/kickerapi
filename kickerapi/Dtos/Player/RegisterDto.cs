using System.ComponentModel.DataAnnotations;

namespace kickerapi.Dtos.Player
{
    public class RegisterDto
    {
        /// <example>test</example>
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        /// <example>Test1*</example>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
