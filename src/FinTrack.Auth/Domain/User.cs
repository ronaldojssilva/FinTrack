namespace FinTrack.Auth.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public static User Create(string name, string email, string passwordHash)
        => new()
        {
            Id           = Guid.NewGuid(),
            Name         = name,
            Email        = email,
            PasswordHash = passwordHash,
            CreatedAt    = DateTime.UtcNow
        };
}
