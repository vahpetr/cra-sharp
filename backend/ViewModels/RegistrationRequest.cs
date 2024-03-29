using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels {
    public class RegistrationRequest {
        [Required, MaxLength (128), EmailAddress]
        public string Email { get; set; }

        [Required, MinLength (8), MaxLength (64), DataType (DataType.Password)]
        public string Password { get; set; }
    }
}