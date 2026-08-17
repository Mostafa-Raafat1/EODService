using System;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;

namespace EODService.Services
{
    /// <summary>
    /// Provides DPAPI-based encryption and decryption for sensitive configuration values
    /// such as database connection strings and API keys.
    ///
    /// Encrypted values are stored with an "ENC:" prefix so the service can distinguish
    /// between legacy plain-text values (still readable as-is) and encrypted ones.
    /// This makes migration safe and gradual — old plain-text entries continue to work
    /// until the user saves them via the settings form, at which point they are encrypted.
    ///
    /// Scope: DataProtectionScope.LocalMachine — any process running on the same
    /// Windows machine (including the EODService Windows Service) can decrypt the data.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class SecurityService
    {
        /// <summary>
        /// Prefix that marks a value as DPAPI-encrypted.
        /// </summary>
        private const string EncryptedPrefix = "ENC:";

        // ── Encrypt ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Encrypts a plain-text string using Windows DPAPI (LocalMachine scope).
        /// Returns the cipher text as a Base64 string prefixed with "ENC:".
        /// Returns the original value unchanged if it is null or empty.
        /// </summary>
        /// <param name="plainText">The plain-text value to encrypt.</param>
        /// <returns>An encrypted string in the form "ENC:&lt;base64&gt;".</returns>
        public static string Encrypt(string? plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText ?? string.Empty;

            // Already encrypted — do not double-encrypt
            if (plainText.StartsWith(EncryptedPrefix, StringComparison.Ordinal))
                return plainText;

            try
            {
                var plainBytes  = Encoding.UTF8.GetBytes(plainText);
                var cipherBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.LocalMachine);
                return EncryptedPrefix + Convert.ToBase64String(cipherBytes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[SecurityService] Encryption failed: {ex.Message}");
                // Fall back to plain text rather than crashing — log the issue
                return plainText;
            }
        }

        // ── Decrypt ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Decrypts a DPAPI-encrypted string (one that starts with "ENC:").
        /// Returns the original value unchanged if it is not prefixed with "ENC:",
        /// allowing backward-compatible reading of legacy plain-text config values.
        /// Returns an empty string if the input is null or empty.
        /// </summary>
        /// <param name="encryptedValue">The value to decrypt (may be plain-text or "ENC:&lt;base64&gt;").</param>
        /// <returns>The decrypted plain-text string.</returns>
        public static string Decrypt(string? encryptedValue)
        {
            if (string.IsNullOrEmpty(encryptedValue))
                return encryptedValue ?? string.Empty;

            // Not encrypted — return as-is (backward compatible with legacy plain-text values)
            if (!encryptedValue.StartsWith(EncryptedPrefix, StringComparison.Ordinal))
                return encryptedValue;

            try
            {
                var base64      = encryptedValue.Substring(EncryptedPrefix.Length);
                var cipherBytes = Convert.FromBase64String(base64);
                var plainBytes  = ProtectedData.Unprotect(cipherBytes, null, DataProtectionScope.LocalMachine);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"[SecurityService] Decryption failed: {ex.Message}");
                // Return empty string rather than crashing — the caller should handle an empty connection string
                return string.Empty;
            }
        }

        // ── Helper ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns true if the given value is already DPAPI-encrypted (starts with "ENC:").
        /// </summary>
        public static bool IsEncrypted(string? value)
            => !string.IsNullOrEmpty(value) && value.StartsWith(EncryptedPrefix, StringComparison.Ordinal);
    }
}
