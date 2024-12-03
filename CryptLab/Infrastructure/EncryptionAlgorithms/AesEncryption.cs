using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CryptLab.Infrastructure.EncryptionAlgorithms;

public class AesEncryption
{
    public string Encrypt(string plainText, string key, string iv)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Padding = PaddingMode.Zeros;
            aesAlg.Mode = CipherMode.CFB;
            aesAlg.IV = Convert.FromBase64String(iv);
            aesAlg.Key = Convert.FromBase64String(key);
            
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
    }

    public string Decrypt(string cipherText, string key, string iv)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Padding = PaddingMode.Zeros;
            aesAlg.Mode = CipherMode.CFB;
            aesAlg.IV = Convert.FromBase64String(iv);
            aesAlg.Key = Convert.FromBase64String(key);
            
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
    
    
    public static (string key, string iv) GenerateKeyAndIV()
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.GenerateKey();
        aesAlg.GenerateIV();

        return (Convert.ToBase64String(aesAlg.Key), Convert.ToBase64String(aesAlg.IV));
    }
}