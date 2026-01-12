using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CMCS.Services
{
    public class FileEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public FileEncryptionService()
        {
            // Simple encryption key (in real app, store securely!)
            _key = Encoding.UTF8.GetBytes("MySecretKey12345"); // 16 bytes for AES
            _iv = Encoding.UTF8.GetBytes("MySecretIV123456");  // 16 bytes for AES
        }

        public byte[] EncryptFile(byte[] fileContent)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(fileContent, 0, fileContent.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        public byte[] DecryptFile(byte[] encryptedContent)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var msDecrypt = new MemoryStream(encryptedContent))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var msPlain = new MemoryStream())
                {
                    csDecrypt.CopyTo(msPlain);
                    return msPlain.ToArray();
                }
            }
        }

        public bool IsValidFileType(string fileName)
        {
            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
            var extension = Path.GetExtension(fileName).ToLower();
            return Array.Exists(allowedExtensions, ext => ext == extension);
        }

        public bool IsValidFileSize(long fileSizeBytes)
        {
            const long maxSizeBytes = 5 * 1024 * 1024; // 5 MB
            return fileSizeBytes <= maxSizeBytes;
        }
    }
}