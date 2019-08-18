using System;
using System.Security.Claims;

namespace Backend.Services.UserService {
    public class AuthorizationDto {
        public string AccessToken { get; set; }
        public DateTime ExpirationIn { get; set; }
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
    }
}