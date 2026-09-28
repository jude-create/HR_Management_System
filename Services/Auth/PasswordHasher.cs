using System.Security.Cryptography;
using System.Text;

namespace HR_Management_System.Services.Auth;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(
            Encoding.UTF8.GetBytes(password)
        );

        return Convert.ToHexString(bytes);
    }
}