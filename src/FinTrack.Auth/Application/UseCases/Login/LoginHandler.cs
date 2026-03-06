using FinTrack.Auth.Application.Interfaces;
using FinTrack.Auth.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FinTrack.Auth.Application.UseCases.Login;

public record LoginCommand(string Email, string Password);
public record LoginResult(string AccessToken, Guid UserId, string Name);

public class LoginHandler(
    IUserRepository users,
    IPasswordHasher hasher,
    IJwtService jwt,
    ILogger<LoginHandler> logger)
{
    public async Task<LoginResult> HandleAsync(LoginCommand cmd, CancellationToken ct = default)
    {
        logger.LogInformation("Login attempt for {Email}", cmd.Email);

        var user = await users.GetByEmailAsync(cmd.Email, ct);
        if (user is null || !hasher.Verify(cmd.Password, user.PasswordHash))
        {
            logger.LogWarning("Failed login attempt for {Email}", cmd.Email);
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var token = jwt.Generate(user.Id, user.Name, user.Email);

        logger.LogInformation("User {UserId} logged in successfully", user.Id);

        return new LoginResult(token, user.Id, user.Name);
    }
}
