using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services.UserService {
    public class UsersSettings {
        public string ConnectionString { get; set; }
        public int MaxRetryCount { get; set; }
        public int MaxRetryDelay { get; set; }
        public string SecretKey { get; set; }
        public int Lifespan { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string PublicUrl { get; set; }
        public SymmetricSecurityKey SymmetricSecurityKey =>
            new SymmetricSecurityKey (Encoding.ASCII.GetBytes (this.SecretKey));
    }
}