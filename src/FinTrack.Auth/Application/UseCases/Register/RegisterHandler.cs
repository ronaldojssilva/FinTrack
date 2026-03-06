using FinTrack.Auth.Application.Interfaces;
using FinTrack.Auth.Domain.Entities;
using FinTrack.Auth.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FinTrack.Auth.Application.UseCases.Register;

public record RegisterCommand(string Name, string Email, string Password);
public record RegisterResult(Guid UserId);

public class RegisterHandler(
    IUserRepository users,
    IPasswordHasher hasher,
    ILogger<RegisterHandler> logger)
{
    public async Task<RegisterResult> HandleAsync(RegisterCommand cmd, CancellationToken ct = default)
    {
        logger.LogInformation("Registering user {Email}", cmd.Email);

        var existing = await users.GetByEmailAsync(cmd.Email, ct);
        if (existing is not null)
        {
            logger.LogWarning("Registration attempt with duplicate email {Email}", cmd.Email);
            throw new InvalidOperationException("Email already registered.");
        }

        var user = User.Create(cmd.Name, cmd.Email, hasher.Hash(cmd.Password));
        await users.AddAsync(user, ct);

        logger.LogInformation("User {UserId} registered successfully", user.Id);

        return new RegisterResult(user.Id);
    }
}
