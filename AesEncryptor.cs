using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace StrictModeModNS
{
    public class AesEncryptor
    {
        readonly Encoding encoding = Encoding.UTF8;
        readonly AesManaged aes;

        /// <param name="password">Should be at least 32 characters.</param>
        public AesEncryptor(string password)
        {
            const int keySize = 256;
            const int saltSize = 16;
            var derivedBytes = new Rfc2898DeriveBytes(password, saltSize);
            aes = new AesManaged { Padding = PaddingMode.PKCS7, KeySize = keySize, Key = derivedBytes.GetBytes(keySize / 8), IV = derivedBytes.Salt };
        }

        public string Encrypt(string text) => CryptoTransform(text, aes.CreateEncryptor());
        public string Decrypt(string text) => CryptoTransform(text, aes.CreateDecryptor());

        string CryptoTransform(string text, ICryptoTransform cryptoTransform)
        {
            byte[] input = encoding.GetBytes(text);
            var stream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write))
                cryptoStream.Write(input, 0, input.Length);
            return encoding.GetString(stream.ToArray());
        }
    }
}
