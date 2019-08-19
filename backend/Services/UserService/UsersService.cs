using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backend.Data;
using Backend.Data.Models;
using Backend.Services.EmailService;
using Backend.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services.UserService {
    public class UserService : IUserService {
        private readonly IOptions<UsersSettings> _options;
        private readonly IEmailService _emailService;
        private readonly UsersContext _context;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService (
            IOptions<UsersSettings> options,
            IEmailService emailService,
            UsersContext context,
            IDistributedCache cache,
            IMapper mapper,
            ILogger<UserService> logger
        ) {
            this._options = options ??
                throw new ArgumentNullException (nameof (options));
            this._emailService = emailService ??
                throw new ArgumentNullException (nameof (emailService));
            this._context = context ??
                throw new ArgumentNullException (nameof (context));
            this._cache = cache ??
                throw new ArgumentNullException (nameof (cache));
            this._mapper = mapper ??
                throw new ArgumentNullException (nameof (mapper));
            this._logger = logger ??
                throw new ArgumentNullException (nameof (logger));
        }

        public async Task<UserDto> RegistrationAsync (string email, string password, CancellationToken ct) {
            UserService.VaildateEmail (email);
            UserService.VaildatePassword (password);

            var notracking = _context.Users.AsQueryable ().AsNoTracking ();

            if (await notracking.AnyAsync (p => p.Email == email, ct)) {
                throw new ServiceException ($"Email {email} already used.");
            }

            var date = DateTime.UtcNow;
            var user = new User {
                Email = email,
                // TODO add PasswordHasher
                Password = password,
                CreatedAt = date,
                UpdatedAt = date
            };
            await _context.Users.AddAsync (user, ct);

            await _context.SaveChangesAsync (ct);

            _logger.LogInformation (LoggingEvents.AddItem, "Register user {id} start.", user.Id);

            var activationToken = Guid.NewGuid ().ToString ();
            var options = new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays (1)
            };
            await _cache.SetStringAsync (activationToken, email, options, ct);

            try {
                var subject = "Account activation";
                var message = $"{_options.Value.PublicUrl}/registration/activation/{activationToken}";
                _logger.LogDebug ("Sending email with subject {subject} and {message}.", subject, message);

                // ONLY FOR EXAMPLE, REMOVE TRY-CATCH IN REAL PROJECT
                try {
                    await _emailService.SendEmailAsync (email, subject, message, ct);
                } catch (Exception) {
                    Console.WriteLine (message);
                }
            } catch (Exception ex) {
                throw new ServiceException ($"Smtp server problem ({ex.Message}).");
            }

            return _mapper.Map<UserDto> (user);
        }

        public async Task ResendActivationTokenAsync (string email, CancellationToken ct) {
            UserService.VaildateEmail (email);

            var notracking = _context.Users.AsQueryable ().AsNoTracking ();

            var user = await notracking.FirstOrDefaultAsync (p => p.Email == email, ct);
            if (user is null) {
                throw new ServiceNotFoundException ($"Email {email} is not registered.");
            }
            if (user.IsActivated) {
                throw new ServiceException ($"User {email} has been activated.");
            }

            var activationToken = Guid.NewGuid ().ToString ();
            var options = new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays (1)
            };
            await _cache.SetStringAsync (activationToken, email, options, ct);

            _logger.LogInformation (LoggingEvents.UpdateItem, "Resend registration token for {email}.", email);
        }

        public async Task<AuthorizationDto> RegistrationConfirmAsync (Guid activationToken, string authenticationType, CancellationToken ct) {
            using (_logger.BeginScope ("Registration confirm scope")) {
                var key = activationToken.ToString ();
                var email = await _cache.GetStringAsync (key, ct);
                if (email is null) {
                    throw new ServiceNotFoundException ("Activation token expired or incorrect.");
                }

                var tracking = _context.Users.AsQueryable ();
                var user = await tracking.FirstOrDefaultAsync (p => p.Email == email, ct);
                if (user is null) {
                    _logger.LogError (LoggingEvents.UnknownError, "Can't find user by activation token {activationToken}.", activationToken);
                    throw new ServiceException ("Please, register again.");
                }
                if (user.IsActivated) {
                    throw new ServiceException ($"User {user.Id} has been activated.");
                }

                user.IsActivated = true;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync (ct);

                var jit = Guid.NewGuid ().ToString ();
                var (accessToken, expires) = CreateAccessToken (user, jit);
                var claimsPrincipal = CreateClaimsPrincipal (user, jit, authenticationType);
                var authorization = new AuthorizationDto () {
                    AccessToken = accessToken,
                    ClaimsPrincipal = claimsPrincipal,
                    ExpirationIn = expires
                };

                await _cache.RemoveAsync (key, ct);

                _logger.LogInformation (LoggingEvents.UpdateItem, "Register user {id} success.", user.Id);

                return authorization;
            }
        }

        public async Task<UserDto> GetUserAsync (Guid id, CancellationToken ct) {
            _logger.LogInformation (LoggingEvents.GetItem, "Getting user {Id}", id);

            var notracking = _context.Users.AsQueryable ().AsNoTracking ();
            var user = await notracking.ProjectTo<UserDto> ().FirstOrDefaultAsync (p => p.Id == id, ct);
            if (user is null) {
                throw new ServiceNotFoundException ($"User {id} not found.");
            }

            return user;
        }

        public async Task < (string, DateTime) > GetAccessTokenAsync (string email, string password, CancellationToken ct) {
            var notracking = _context.Users.AsQueryable ().AsNoTracking ();

            var user = await notracking.FirstOrDefaultAsync (p => p.Email == email, ct);
            if (user is null) {
                throw new ServiceException ("Incorrect login or password.");
            }
            // TODO add PasswordHasher
            if (user.Password != password) {
                throw new ServiceException ("Incorrect login or password.");
            }
            if (!user.IsActivated) {
                throw new ServiceException ($"User {email} is not confirmed.");
            }

            var jit = Guid.NewGuid ().ToString ();
            var (accessToken, expires) = CreateAccessToken (user, jit);

            _logger.LogInformation (LoggingEvents.GetItem, "Get access token {email}.", email);

            return (accessToken, expires);
        }

        public async Task<ClaimsPrincipal> GetClaimsPrincipalAsync (string email, string password, string authenticationType, CancellationToken ct) {
            var notracking = _context.Users.AsQueryable ().AsNoTracking ();
            var user = await notracking.FirstOrDefaultAsync (p => p.Email == email, ct);
            if (user is null) {
                throw new ServiceException ("Incorrect login or password.");
            }
            // TODO add PasswordHasher
            if (user.Password != password) {
                throw new ServiceException ("Incorrect login or password.");
            }
            if (!user.IsActivated) {
                throw new ServiceException ($"User {email} is not confirmed.");
            }

            var jit = Guid.NewGuid ().ToString ();
            var principal = CreateClaimsPrincipal (user, jit, authenticationType);

            _logger.LogInformation (LoggingEvents.UpdateItem, "Get claims principal user {id} success.", user.Id);

            return principal;
        }

        public async Task<AuthorizationDto> GetAuthorizationAsync (string email, string password, string authenticationType, CancellationToken ct) {
            var notracking = _context.Users.AsQueryable ().AsNoTracking ();

            var user = await notracking.FirstOrDefaultAsync (p => p.Email == email, ct);
            if (user is null) {
                throw new ServiceException ("Incorrect login or password.");
            }
            // TODO add PasswordHasher
            if (user.Password != password) {
                throw new ServiceException ("Incorrect login or password.");
            }
            if (!user.IsActivated) {
                throw new ServiceException ($"User {email} is not confirmed.");
            }

            var jit = Guid.NewGuid ().ToString ();
            var (accessToken, expires) = CreateAccessToken (user, jit);
            var claimsPrincipal = CreateClaimsPrincipal (user, jit, authenticationType);
            var authorization = new AuthorizationDto () {
                AccessToken = accessToken,
                ClaimsPrincipal = claimsPrincipal,
                ExpirationIn = expires
            };

            _logger.LogInformation (LoggingEvents.GetItem, "User login {email}.", email);

            return authorization;
        }

        public async Task<T> CreateTransactionAsync<T> (Func<Task<T>> action, CancellationToken ct) {
            return await ResilientTransaction.New (_context).ExecuteAsync (action, ct);
        }

        private Claim[] CreateClaims (User user, string jit) {
            // https://ru.wikipedia.org/wiki/JSON_Web_Token
            return new Claim[] {
                new Claim (JwtRegisteredClaimNames.Jti, jit),
                    new Claim (JwtRegisteredClaimNames.Sub, user.Id.ToString ()),
                    new Claim (JwtRegisteredClaimNames.Email, user.Email),
                    new Claim (ClaimTypes.Role, "User"),
            };
        }

        private ClaimsPrincipal CreateClaimsPrincipal (User user, string jit, string authenticationType) {
            var claims = CreateClaims (user, jit);
            var claimsIdentity = new ClaimsIdentity (claims, authenticationType);
            return new ClaimsPrincipal (claimsIdentity);
        }

        private (string, DateTime) CreateAccessToken (User user, string jit) {
            var userSettings = _options.Value;
            var notBefore = DateTime.UtcNow;
            var expires = notBefore + TimeSpan.FromMinutes (userSettings.Lifespan);
            var claims = CreateClaims (user, jit);
            var signingCredentials = new SigningCredentials (userSettings.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken (
                issuer: userSettings.Issuer,
                audience: userSettings.Audience,
                notBefore: notBefore,
                claims: claims,
                expires: expires,
                signingCredentials: signingCredentials
            );
            var accessToken = new JwtSecurityTokenHandler ().WriteToken (jwtSecurityToken);
            return (accessToken, expires);
        }

        static void VaildatePassword (string password) {
            // TODO add validation https://stackoverflow.com/a/40910204/3074281
            // example validation
            if (string.IsNullOrWhiteSpace (password)) {
                throw new ServiceException ("Password must be fill.");
            }

            if (password.Equals ("bad password")) {
                throw new ServiceException ("Password must be good.");
            }
        }

        static void VaildateEmail (string email) {
            // example validation
            if (string.IsNullOrWhiteSpace (email)) {
                throw new ServiceException ("Email must be fill.");
            }

            if (email.Equals ("not@email.unknown")) {
                throw new ServiceException ("Email must be email.");
            }
        }
    }
}