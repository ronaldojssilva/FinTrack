namespace FinTrack.Auth.Application.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

public interface IJwtService
{
    string Generate(Guid userId, string name, string email);
}
