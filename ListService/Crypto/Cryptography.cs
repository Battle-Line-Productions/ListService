namespace ListService.Crypto
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using Extensions;
    using Interfaces;

    public class Cryptography : ICryptography
    {
        private readonly string _encryptionKey;

        private const int BlockBitSize = 128;
        private const int KeyBitSize = 256;

        public Cryptography(string encryptionKey)
        {
            _encryptionKey = encryptionKey;
        }


        public string Encrypt(string toEncrypt)
        {
            using var aesManaged = new AesManaged { KeySize = KeyBitSize, BlockSize = BlockBitSize };
            aesManaged.GenerateIV();

            var keyBytes = _encryptionKey.HexStringToByteArray();
            var ivBytes = aesManaged.IV;

            using var encryptor = aesManaged.CreateEncryptor(keyBytes, ivBytes);
            using var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(toEncrypt);
            }

            var cipherTextBytes = memoryStream.ToArray();

            Array.Resize(ref ivBytes, ivBytes.Length + cipherTextBytes.Length);
            Array.Copy(cipherTextBytes, 0, ivBytes, BlockBitSize / 8, cipherTextBytes.Length);

            return Convert.ToBase64String(ivBytes);
        }

        public string Decrypt(string toDecrypt)
        {
            var ivBytes = new byte[BlockBitSize / 8];
            var keyBytes = _encryptionKey.HexStringToByteArray();

            var allTheBytes = Convert.FromBase64String(toDecrypt);

            Array.Copy(allTheBytes, 0, ivBytes, 0, ivBytes.Length);

            var cipherTextBytes = new byte[allTheBytes.Length - ivBytes.Length];
            Array.Copy(allTheBytes, ivBytes.Length, cipherTextBytes, 0, cipherTextBytes.Length);

            using var aesManaged = new AesManaged {KeySize = KeyBitSize, BlockSize = BlockBitSize};
            using var decryptor = aesManaged.CreateDecryptor(keyBytes, ivBytes);
            using var memoryStream = new MemoryStream(cipherTextBytes);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }
    }
}