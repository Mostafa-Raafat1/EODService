using System;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace EODService.Services
{
    /// <summary>
    /// Manages the shared AES-256 encryption key on the local machine.
    ///
    /// The shared key is derived from a user passphrase using PBKDF2 (Rfc2898DeriveBytes).
    /// The derived 256-bit key is then stored in C:\EODConfig\security.dat, protected
    /// by Windows DPAPI (LocalMachine scope).
    ///
    /// This guarantees:
    /// 1. All machines running EODService share the SAME AES key (from identical passphrase).
    /// 2. The key file on disk is encrypted by DPAPI, so unauthorized users on the machine cannot steal the key.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class KeyStoreService
    {
        private static readonly string ConfigDirectory = @"C:\EODConfig";
        private static readonly string KeyFilePath = Path.Combine(ConfigDirectory, "security.dat");

        // Fixed salt for key derivation across devices so identical passphrases generate identical AES keys
        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("EODService_Shared_Salt_2026");
        private const int Iterations = 10_000;

        /// <summary>
        /// Checks if the local machine has a configured key file.
        /// </summary>
        public static bool KeyExists()
        {
            return File.Exists(KeyFilePath);
        }

        /// <summary>
        /// Derives a 256-bit key from a passphrase and saves it DPAPI-encrypted to disk.
        /// </summary>
        public static void SaveKey(string passphrase)
        {
            if (string.IsNullOrWhiteSpace(passphrase))
                throw new ArgumentException("Passphrase cannot be empty.", nameof(passphrase));

            if (!Directory.Exists(ConfigDirectory))
            {
                Directory.CreateDirectory(ConfigDirectory);
            }

            // Derive 32-byte (256-bit) AES key using PBKDF2
            using var deriveBytes = new Rfc2898DeriveBytes(passphrase, Salt, Iterations, HashAlgorithmName.SHA256);
            var key = deriveBytes.GetBytes(32);

            // Protect with DPAPI (LocalMachine scope)
            var protectedKey = ProtectedData.Protect(key, null, DataProtectionScope.LocalMachine);

            File.WriteAllBytes(KeyFilePath, protectedKey);
        }

        /// <summary>
        /// Reads and decrypts the AES key from disk.
        /// Returns null if key file does not exist.
        /// </summary>
        public static byte[]? GetKey()
        {
            if (!KeyExists())
                return null;

            try
            {
                var protectedKey = File.ReadAllBytes(KeyFilePath);
                return ProtectedData.Unprotect(protectedKey, null, DataProtectionScope.LocalMachine);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[KeyStoreService] GetKey failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Verifies whether the provided passphrase matches the current key stored on disk.
        /// </summary>
        public static bool VerifyPassphrase(string passphrase)
        {
            var currentKey = GetKey();
            if (currentKey == null)
                return false;

            var candidateKey = AesEncryptionService.DeriveKey(passphrase);
            return CryptographicOperations.FixedTimeEquals(currentKey, candidateKey);
        }
    }
}
