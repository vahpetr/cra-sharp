using System;
using System.Threading;
using System.Threading.Tasks;
using Backend.Services.UserService;
using Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers {
    [ApiController]
    [Route ("api/users")]
    public class UsersApiController : ControllerBase {
        private readonly IUserService _userService;
        private readonly ILogger<UsersApiController> _logger;

        public UsersApiController (
            IUserService userService,
            ILogger<UsersApiController> logger
        ) {
            this._userService = userService ??
                throw new ArgumentNullException (nameof (UserService));
            this._logger = logger ??
                throw new ArgumentNullException (nameof (logger));
        }

        [Authorize]
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
                var accessToken = await _userService.RegistrationConfirmAsync (vm.ActivationToken, ct);
                return Ok (new AuthorizationViewModel { AccessToken = accessToken });
            }, ct);
        }

        [HttpPost ("registration/resend-token")]
        public async Task<ActionResult> PostResendActivationTokenAsync ([FromBody] ResendActivationTokenRequest vm, CancellationToken ct) {
            if (!ModelState.IsValid) {
                return BadRequest (new { error = "Incorrect parameters.", errors = ModelState });
            }

            await _userService.ResendRegistrationConfirmAsync (vm.Email, ct);
            return Ok ();
        }

        [HttpPost ("login")]
        public async Task<ActionResult<AuthorizationViewModel>> GetLoginAsync ([FromBody] AuthenticationRequest vm, CancellationToken ct) {
            if (!ModelState.IsValid) {
                return BadRequest (new { error = "Incorrect parameters.", errors = ModelState });
            }

            var accessToken = await _userService.GetAccessTokenAsync (vm.Email, vm.Password, ct);
            return Ok (new AuthorizationViewModel { AccessToken = accessToken });
        }
    }
}