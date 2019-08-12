using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels {
    public class ResendActivationTokenRequest {
        [Required, MaxLength (128), EmailAddress]
        public string Email { get; set; }
    }
}