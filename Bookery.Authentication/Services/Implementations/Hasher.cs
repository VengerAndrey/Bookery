using System.Security.Cryptography;
using System.Text;
using Bookery.Authentication.Services.Interfaces;

namespace Bookery.Authentication.Services.Implementations;

public class Hasher : IHasher
{
    private readonly string _salt;

    public Hasher(string salt)
    {
        _salt = salt;
    }

    public string Hash(string input)
    {
        using HashAlgorithm sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input + _salt));
        var stringBuilder = new StringBuilder();
        foreach (var b in bytes)
        {
            stringBuilder.Append(b.ToString("X2"));
        }

        return stringBuilder.ToString();
    }
}