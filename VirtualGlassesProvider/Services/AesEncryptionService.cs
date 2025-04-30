using System.Security.Cryptography;


namespace VirtualGlassesProvider.Services
{
    public class AesEncryptionService(
        string key, 
        string iv)
    {
        private readonly byte[] _key = Convert.FromBase64String(key);
        private readonly byte[] _iv = Convert.FromBase64String(iv);


        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using var sw = new StreamWriter(cs);
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }


        public string Decrypt(string cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
