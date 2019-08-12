using System;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Services.UserService {
    public interface IUserService {
        Task<UserDto> RegistrationAsync (string email, string password, CancellationToken ct);
        Task<string> RegistrationConfirmAsync (Guid activationToken, CancellationToken ct);
        Task ResendRegistrationConfirmAsync (string email, CancellationToken ct);
        Task<string> GetAccessTokenAsync (string email, string password, CancellationToken ct);
        Task<UserDto> GetUserAsync (Guid id, CancellationToken ct);
        Task<T> CreateTransactionAsync<T> (Func<Task<T>> action, CancellationToken ct);
    }
}