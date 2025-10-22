using Microsoft.AspNetCore.Identity;

public class PasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new PasswordHasher<object>();

    public string HashPassword(string password)
    {
        return _hasher.HashPassword(null, password);
    }
}