using System.Security.Cryptography;
using System.Text;

namespace RaceDay.Api.Filters;

public static class PasswordHasher
{
    // The salt is fine to hardcode for this project — a real system would
    // store a unique random salt per user in the database.
    private const string Salt = "RaceDay_Static_Salt_v1";

    public static string Hash(string password)
    {
        using var sha = SHA256.Create();
        var combined = password + Salt;
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(combined));
        return Convert.ToBase64String(bytes);
    }

    public static bool Verify(string password, string storedHash)
    {
        var computed = Hash(password);
        // Constant-time comparison to avoid timing attacks
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computed),
            Encoding.UTF8.GetBytes(storedHash));
    }
}