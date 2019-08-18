using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Services.UserService {
    public interface IUserService {
        Task<UserDto> RegistrationAsync (string email, string password, CancellationToken ct);
        Task ResendActivationTokenAsync (string email, CancellationToken ct);
        Task<AuthorizationDto> RegistrationConfirmAsync (Guid activationToken, string authenticationType, CancellationToken ct);
        Task<UserDto> GetUserAsync (Guid id, CancellationToken ct);
        Task < (string, DateTime) > GetAccessTokenAsync (string email, string password, CancellationToken ct);
        Task<ClaimsPrincipal> GetClaimsPrincipalAsync (string email, string password, string authenticationType, CancellationToken ct);
        Task<AuthorizationDto> GetAuthorizationAsync (string email, string password, string authenticationType, CancellationToken ct);
        Task<T> CreateTransactionAsync<T> (Func<Task<T>> action, CancellationToken ct);
    }
}