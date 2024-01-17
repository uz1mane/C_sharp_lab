using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Lab_1
{
    public class JwtConfigurations
    {
        private static Random random = new Random();

        public const string Issuer = "JwtIssuer"; // издатель токена
        public const string Audience = "JwtClient"; // потребитель токена
        public static string Key = "Lab_1_cipher_key";   // ключ для шифрации
        public static readonly TimeSpan Lifetime = new TimeSpan(0, 1, 0, 0, 0); // время жизни токена - 60 минут
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }

        public static void RuinAllTokens()
        {
            Key = RandomString(16);
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
