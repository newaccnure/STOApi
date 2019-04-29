using Microsoft.IdentityModel.Tokens;
using System.Text;
 
namespace STOApi.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "TestDotnetServer"; // издатель токена
        public const string AUDIENCE = "https://localhost:5001";
        const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
        public const int LIFETIME = 1000; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}