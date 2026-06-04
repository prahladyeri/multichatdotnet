using System;
using System.Security.Cryptography;
using System.Text;

namespace multichatdotnet.Helpers
{
    public static class DpapiSecretVault
    {
        private static readonly byte[] OptionalEntropy = Encoding.UTF8.GetBytes("xDoceQiyrdLkz");

        public static string Encrypt(string plainText, DataProtectionScope scope)
        {
            if (string.IsNullOrEmpty(plainText)) return null;

            // Convert string to bytes
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            // Encrypt the data using DPAPI
            byte[] encryptedBytes = ProtectedData.Protect(plainBytes, OptionalEntropy, scope);

            // Return as Base64 string for easy storage in config files or databases
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Decrypt(string encryptedText, DataProtectionScope scope)
        {
            if (string.IsNullOrEmpty(encryptedText)) return null;

            try
            {
                // Convert Base64 string back to bytes
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

                // Decrypt the data using DPAPI
                byte[] plainBytes = ProtectedData.Unprotect(encryptedBytes, OptionalEntropy, scope);

                // Return as string
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (CryptographicException e)
            {
                // Thrown if the scope is wrong, user changed, or data was tampered with
                Console.WriteLine($"Decryption failed: {e.Message}");
                return null;
            }
        }
    }

}
