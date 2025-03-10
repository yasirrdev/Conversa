using Conversa.Models.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Conversa.Services;

public class PasswordService : IPasswordHasher
{
    public string Hash(string password)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(password);
        byte[] inputHash = SHA256.HashData(inputBytes);
        return Convert.ToBase64String(inputHash);
    }
}
