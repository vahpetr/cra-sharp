using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Backend.Services.UserService;
using Backend.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers {
    [ApiController]
    [Route ("api/users")]
    [Produces ("application/json")]
    public class UsersApiController : ControllerBase {
        private readonly IUserService _userService;
        private readonly ILogger<UsersApiController> _logger;

        public UsersApiController (
            IUserService userService,
            ILogger<UsersApiController> logger
        ) {
            this._userService = userService ??
                throw new ArgumentNullException (nameof (userService));
            this._logger = logger ??
                throw new ArgumentNullException (nameof (logger));
        }

        [Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost ("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetAsync (Guid id, CancellationToken ct) {
            return await _userService.GetUserAsync (id, ct);
        }

        [HttpPost ("registration")]
        public async Task<ActionResult> PostRegistrationAsync ([FromBody] RegistrationRequest vm, CancellationToken ct) {
            if (!ModelState.IsValid) {
                return BadRequest (new { error = "Incorrect parameters.", errors = ModelState });
            }

            return await _userService.CreateTransactionAsync (async () => {
                var user = await _userService.RegistrationAsync (vm.Email, vm.Password, ct);
                return CreatedAtAction (nameof (GetAsync), new { id = user.Id }, user);
            }, ct);
        }

        [HttpPost ("registration/confirm")]
        public async Task<ActionResult<AuthorizationViewModel>> PostRegistrationConfirmAsync ([FromBody] RegistrationConfirmRequest vm, CancellationToken ct) {
            return await _userService.CreateTransactionAsync (async () => {
                var schema = CookieAuthenticationDefaults.AuthenticationScheme;
                var authorization = await _userService.RegistrationConfirmAsync (vm.ActivationToken, schema, ct);
                var rememberMe = true;
                await SignInAsync (authorization.ClaimsPrincipal, rememberMe, authorization.ExpirationIn);
                return Ok (new AuthorizationViewModel { AccessToken = authorization.AccessToken });
            }, ct);
        }

        [HttpPost ("registration/resend-activation-token")]
        public async Task<ActionResult> PostResendActivationTokenAsync ([FromBody] ResendActivationTokenRequest vm, CancellationToken ct) {
            if (!ModelState.IsValid) {
                return BadRequest (new { error = "Incorrect parameters.", errors = ModelState });
            }

            await _userService.ResendActivationTokenAsync (vm.Email, ct);
            return Ok ();
        }

        [HttpPost ("login")]
        public async Task<ActionResult<AuthorizationViewModel>> GetLoginAsync ([FromBody] AuthenticationRequest vm, CancellationToken ct) {
            if (!ModelState.IsValid) {
                return BadRequest (new { error = "Incorrect parameters.", errors = ModelState });
            }
            var schema = CookieAuthenticationDefaults.AuthenticationScheme;
            var authorization = await _userService.GetAuthorizationAsync (vm.Email, vm.Password, schema, ct);
            await SignInAsync (authorization.ClaimsPrincipal, vm.RememberMe, authorization.ExpirationIn);
            return Ok (new AuthorizationViewModel { AccessToken = authorization.AccessToken });
        }

        [HttpPost ("logout")]
        public async Task LogoutAsync () {
            // SignOutAsync not support CancellationToken
            await HttpContext.SignOutAsync (CookieAuthenticationDefaults.AuthenticationScheme);
            // TODO add canceling jwt token https://piotrgankiewicz.com/2018/04/25/canceling-jwt-tokens-in-net-core/
        }

        private async Task SignInAsync (ClaimsPrincipal claimsPrincipal, bool isPersistent, DateTime expiresUtc) {
            // SignInAsync not support CancellationToken
            await HttpContext.SignInAsync (
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                new AuthenticationProperties {
                    IsPersistent = isPersistent,
                        ExpiresUtc = expiresUtc
                });
        }
    }
}