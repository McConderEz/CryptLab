using System.Security.Cryptography;
using System.Text;

namespace CryptLab.Infrastructure.EncryptionAlgorithms;

public static class Sha1HashCalculate
{
    public static string ComputeSha1Hash(string rawData)
    {
        using SHA1 sha1Hash = SHA1.Create();
        byte[] bytes = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        
        StringBuilder builder = new StringBuilder();
        foreach (byte b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        
        return builder.ToString();
    }
}