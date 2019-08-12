using System;
using System.ComponentModel.DataAnnotations;

namespace Backend.ViewModels {
    public class RegistrationConfirmRequest {
        public Guid ActivationToken { get; set; }
    }
}