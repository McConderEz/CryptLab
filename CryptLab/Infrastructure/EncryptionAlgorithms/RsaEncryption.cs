using System.Security.Cryptography;
using System.Text;

namespace CryptLab.Infrastructure.EncryptionAlgorithms;

public class RsaEncryption
{
    private RSACryptoServiceProvider _provider;

    public RsaEncryption()
    {
        _provider = new RSACryptoServiceProvider();
    }

    public string Encrypt(string plainText)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encryptedBytes = _provider.Encrypt(plainBytes, false);

        return Convert.ToBase64String(encryptedBytes);
    }
    
    public string Decrypt(string encryptedText)
    {
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        byte[] plainBytes = _provider.Decrypt(encryptedBytes, false);
        return Encoding.UTF8.GetString(plainBytes);
    }

    public (string, string) GenerateKeys()
    {
        var privateKey = _provider.ToXmlString(true);
        var publicKey = _provider.ToXmlString(false);
        
        return (privateKey, publicKey);
    }

    public void LoadKeys(string key)
    {
        try
        {
            _provider.FromXmlString(key);
        }
        catch (CryptographicException ex)
        {
            throw new Exception("Ошибка загрузки ключей", ex);
        }
    }
    
}