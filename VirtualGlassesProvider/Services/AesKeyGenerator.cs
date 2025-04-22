using System.Security.Cryptography;
namespace VirtualGlassesProvider.Services
{
    public static class AesKeyGenerator
    {
        public static (string Key, string IV) GenerateAesKeyAndIV()
        {
            using var aes = Aes.Create();
            aes.GenerateKey(); // Generates a random key
            aes.GenerateIV(); // Generates a random IV
            string base64Key = Convert.ToBase64String(aes.Key);
            string base64IV = Convert.ToBase64String(aes.IV);
            return (base64Key, base64IV);
        }
    }
}
