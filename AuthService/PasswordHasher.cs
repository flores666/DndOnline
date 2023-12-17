using System.Security.Cryptography;

namespace AuthService;

public static class PasswordHasher
{
    private const int _saltSize = 16;
    private const int _keySize = 32;
    private const int _iterationsNumber = 1000;

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(_saltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 
            _iterationsNumber, HashAlgorithmName.SHA256, _keySize);
        return string.Join(".", Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public static bool Validate(string password, string passwordHash)
    {
        var pwdElements = passwordHash.Split(".");
        var salt = Convert.FromBase64String(pwdElements[0]);
        var hash = Convert.FromBase64String(pwdElements[1]);
        var hashInput = Rfc2898DeriveBytes.Pbkdf2(password, salt, 
            _iterationsNumber, HashAlgorithmName.SHA256, _keySize);

        return CryptographicOperations.FixedTimeEquals(hash, hashInput);
    }
}