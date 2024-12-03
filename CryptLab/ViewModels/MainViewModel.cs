using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using CryptLab.Infrastructure.Command;
using CryptLab.Infrastructure.EncryptionAlgorithms;
using CryptLab.Infrastructure.Extensions;
using CryptLab.Infrastructure.ViewModel;
using Microsoft.Win32;

namespace CryptLab.ViewModels;

public class MainViewModel: ViewModel
{
    private const string ENCRYPTED_TEXT_PATH = "textEncrypted.txt";
    private const string AES_KEY_PATH = "keyEncrypted.txt";
    private const string AES_IV_PATH = "ivEncrypted.txt";
    private const string RSA_PUBLIC_KEY_PATH = "rsaPublicKey.txt";
    private const string RSA_PRIVATE_KEY_PATH = "rsaPrivateKey.txt";
    private const string HASH_PATH = "hash.txt";
    private const string SIGNATURE_PATH = "signature.txt";
    private const string DSA_PARAMETERS_PATH = "dsaParams.json";
    
    private string _text;
    private string _decryptedText;
    private string _iv;
    private string _key;

    public string Text
    {
        get => _text;
        set => Set(ref _text, value);
    }

    public string DecryptedText
    {
        get => _decryptedText;
        set => Set(ref _decryptedText, value);
    }

    public string IV
    {
        get => _iv;
        set => Set(ref _iv, value);
    }
    
    public string Key
    {
        get => _key;
        set => Set(ref _key, value);
    }

    #region LoadTextFromFile

    public ICommand LoadTextFromFileCommand { get; }

    private async void OnLoadTextFromFileCommandExecute(object p)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
            Title = "Загрузить файл"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            string filePath = openFileDialog.FileName;
            
            using var sr = new StreamReader(filePath);

            Text = await sr.ReadToEndAsync();
        }
    }

    private bool CanLoadTextFromFileCommandExecute(object p) => true;

    #endregion

    #region Encrypt
    public ICommand EncryptCommand { get; }

    private async void OnEncryptCommandExecute(object p)
    {
        try
        {
            (Key, IV) = AesEncryption.GenerateKeyAndIV();

            AesEncryption aes = new AesEncryption();
            var encryptedText = aes.Encrypt(_text, Key, IV);

            RsaEncryption rsa = new RsaEncryption();

            (string privateKey, string publicKey) = rsa.GenerateKeys();
            rsa.LoadKeys(publicKey);

            var keyEncrypted = rsa.Encrypt(_key);
            var ivEncrypted = rsa.Encrypt(_iv);

            var hash = Sha1HashCalculate.ComputeSha1Hash(Text);
            var dsaParameters = DsaEncryption.GenerateParams();

            var signature = DsaEncryption.SignMessage(hash, dsaParameters);

            var jsonDsaParams = JsonSerializer.Serialize(dsaParameters, new JsonSerializerOptions
            {
                Converters = { new DSAParametersConverter() }
            });

            await File.WriteAllTextAsync(ENCRYPTED_TEXT_PATH, encryptedText);
            await File.WriteAllTextAsync(AES_KEY_PATH, keyEncrypted);
            await File.WriteAllTextAsync(AES_IV_PATH, ivEncrypted);
            await File.WriteAllTextAsync(RSA_PUBLIC_KEY_PATH, publicKey);
            await File.WriteAllTextAsync(RSA_PRIVATE_KEY_PATH, privateKey);
            await File.WriteAllTextAsync(SIGNATURE_PATH, signature);
            await File.WriteAllTextAsync(HASH_PATH, hash);
            await File.WriteAllTextAsync(DSA_PARAMETERS_PATH, jsonDsaParams);
        }
        catch (ArgumentNullException ex)
        {
            MessageBox.Show("Возможно вы забыли внести данные для шифрования");
        }
    }

    private bool CanEncryptCommandExecute(object p) => true;
    
    #endregion
    
    #region Decrypt
    public ICommand DecryptCommand { get; }

    private async void OnDecryptCommandExecute(object p)
    {
        try
        {
            var encryptedText = await File.ReadAllTextAsync(ENCRYPTED_TEXT_PATH);
            var keyEncrypted = await File.ReadAllTextAsync(AES_KEY_PATH);
            var ivEncrypted = await File.ReadAllTextAsync(AES_IV_PATH);
            var privateKey = await File.ReadAllTextAsync(RSA_PRIVATE_KEY_PATH);
            var signature = await File.ReadAllTextAsync(SIGNATURE_PATH);
            var stringDsaParams = await File.ReadAllTextAsync(DSA_PARAMETERS_PATH);

            var dsaParameters = JsonSerializer.Deserialize<DSAParameters>(stringDsaParams, new JsonSerializerOptions
            {
                Converters = { new DSAParametersConverter() }
            });

            RsaEncryption rsa = new RsaEncryption();
            rsa.LoadKeys(privateKey);

            var key = rsa.Decrypt(keyEncrypted);
            var iv = rsa.Decrypt(ivEncrypted);

            AesEncryption aes = new AesEncryption();
            DecryptedText = aes.Decrypt(encryptedText, key, iv);
        }
        catch (ArgumentNullException ex)
        {
            MessageBox.Show("Отсутствуют файлы для расшифровки данных");
        }

    }

    private bool CanDecryptCommandExecute(object p) => true;
    

    #endregion
    
    #region Verify
    public ICommand VerifyCommand { get; }

    private async void OnVerifyCommandExecute(object p)
    {
        try
        {
            var signature = await File.ReadAllTextAsync(SIGNATURE_PATH);
            var stringDsaParams = await File.ReadAllTextAsync(DSA_PARAMETERS_PATH);

            var dsaParameters = JsonSerializer.Deserialize<DSAParameters>(stringDsaParams, new JsonSerializerOptions
            {
                Converters = { new DSAParametersConverter() }
            });

            var hash = Sha1HashCalculate.ComputeSha1Hash(DecryptedText);

            var result = DsaEncryption.VerifySignature(
                hash,
                Convert.FromBase64String(signature),
                dsaParameters);

            if (result)
                MessageBox.Show("Верная сигнатура");
            else
                MessageBox.Show("Неверная сигнатура");
        }
        catch (ArgumentNullException ex)
        {
            MessageBox.Show("Отсутствуют данные для верификации");
        }
    }

    private bool CanVerifyCommandExecute(object p) => true;
    

    #endregion

    
    public MainViewModel()
    {
        LoadTextFromFileCommand = new LambdaCommand(OnLoadTextFromFileCommandExecute);
        EncryptCommand = new LambdaCommand(OnEncryptCommandExecute);
        DecryptCommand = new LambdaCommand(OnDecryptCommandExecute);
        VerifyCommand = new LambdaCommand(OnVerifyCommandExecute);
    }
}