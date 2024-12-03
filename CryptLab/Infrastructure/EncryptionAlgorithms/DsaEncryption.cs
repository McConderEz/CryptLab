using System.Security.Cryptography;
using System.Text;

namespace CryptLab.Infrastructure.EncryptionAlgorithms;

public class DsaEncryption
{
    
    public static string SignMessage(
        string message,
        DSAParameters dsaParameters)
    {
        using DSA dsa = DSA.Create();
        dsa.ImportParameters(dsaParameters);
        byte[] messageBytes = Convert.FromBase64String(message);
        var sign = dsa.SignData(messageBytes, HashAlgorithmName.SHA1, DSASignatureFormat.Rfc3279DerSequence);
        return Convert.ToBase64String(sign);
    }

    
    public static bool VerifySignature(
        string message, 
        byte[] signature,
        DSAParameters dsaParameters)
    {
        using DSA dsa = DSA.Create();
        dsa.ImportParameters(dsaParameters);
        byte[] messageBytes = Convert.FromBase64String(message);
        return dsa.VerifyData(messageBytes, signature, HashAlgorithmName.SHA1, DSASignatureFormat.Rfc3279DerSequence);
    }
    
    public static DSAParameters GenerateParams()
    {
        using DSA dsa = DSA.Create();
        return dsa.ExportParameters(true);
    }
}