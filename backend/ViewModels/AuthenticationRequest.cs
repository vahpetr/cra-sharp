using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels {
    public class AuthenticationRequest {
        [Required, MaxLength (128), EmailAddress]
        public string Email { get; set; }

        [Required, MinLength (8), MaxLength (64), DataType (DataType.Password)]
        public string Password { get; set; }

        [DefaultValue(true)]
        public bool RememberMe { get; set; }
    }
}