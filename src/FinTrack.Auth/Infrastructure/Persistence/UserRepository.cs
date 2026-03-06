using FinTrack.Auth.Domain.Entities;
using FinTrack.Auth.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Auth.Infrastructure.Persistence;

public class UserRepository(AuthDbContext db) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
    }
}
