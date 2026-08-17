using System;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace EODService.Services
{
    /// <summary>
    /// Provides AES-256 symmetric encryption and decryption for values shared
    /// across multiple machines (such as API keys stored in the Oracle DB).
    ///
    /// Stored format: "AES:<base64_IV>:<base64_CipherText>"
    ///
    /// Each encryption call generates a random IV to prevent deterministic cipher text.
    /// Legacy plain-text values (without the "AES:" prefix) pass through un-encrypted
    /// for backward compatibility.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class AesEncryptionService
    {
        private const string Prefix = "AES:";

        /// <summary>
        /// Encrypts a plain-text string using AES-256 with a shared 32-byte key.
        /// </summary>
        public static string Encrypt(string? plainText, byte[] key)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText ?? string.Empty;

            if (IsAesEncrypted(plainText))
                return plainText;

            try
            {
                using var aes = Aes.Create();
                aes.KeySize = 256;
                aes.Key = key;
                aes.GenerateIV();

                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream();
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cs, Encoding.UTF8))
                {
                    writer.Write(plainText);
                }

                var cipherBytes = ms.ToArray();
                var ivBase64 = Convert.ToBase64String(aes.IV);
                var cipherBase64 = Convert.ToBase64String(cipherBytes);

                return $"{Prefix}{ivBase64}:{cipherBase64}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[AesEncryptionService] Encrypt error: {ex.Message}");
                return plainText;
            }
        }

        /// <summary>
        /// Decrypts an AES-256 encrypted string ("AES:<iv>:<cipher>").
        /// Returns legacy plain-text strings unchanged.
        /// </summary>
        public static string Decrypt(string? encryptedValue, byte[] key)
        {
            if (string.IsNullOrEmpty(encryptedValue))
                return encryptedValue ?? string.Empty;

            if (!IsAesEncrypted(encryptedValue))
                return encryptedValue;

            try
            {
                var raw = encryptedValue.Substring(Prefix.Length);
                var parts = raw.Split(':');
                if (parts.Length != 2)
                    return encryptedValue;

                var iv = Convert.FromBase64String(parts[0]);
                var cipherBytes = Convert.FromBase64String(parts[1]);

                using var aes = Aes.Create();
                aes.KeySize = 256;
                aes.Key = key;
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(cipherBytes);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var reader = new StreamReader(cs, Encoding.UTF8);

                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[AesEncryptionService] Decrypt error: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Checks if a string is AES-encrypted.
        /// </summary>
        public static bool IsAesEncrypted(string? value)
            => !string.IsNullOrEmpty(value) && value.StartsWith(Prefix, StringComparison.Ordinal);
    }
}
