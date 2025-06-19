using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

public class AesCFB128Encryptor
{
    public static string Encrypt(string plainText, string keyStr)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(keyStr);
        byte[] ivBytes = keyBytes; // IV 和 key 相同
        byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);

        // AES CFB128 无填充
        IBufferedCipher cipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 128));
        cipher.Init(true, new ParametersWithIV(new KeyParameter(keyBytes), ivBytes));

        byte[] encryptedBytes = cipher.DoFinal(inputBytes);

        return Convert.ToBase64String(encryptedBytes);
    }

    public static string Decrypt(string base64CipherText, string keyStr)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(keyStr);
        byte[] ivBytes = keyBytes; // IV 与 key 相同
        byte[] cipherBytes = Convert.FromBase64String(base64CipherText);

        // AES CFB128 无填充
        IBufferedCipher cipher = new BufferedBlockCipher(new CfbBlockCipher(new AesEngine(), 128));
        cipher.Init(false, new ParametersWithIV(new KeyParameter(keyBytes), ivBytes));

        byte[] decryptedBytes = cipher.DoFinal(cipherBytes);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
