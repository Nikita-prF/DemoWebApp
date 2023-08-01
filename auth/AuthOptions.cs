using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebHelloApp.auth
{
    /// <summary>
    /// Класс, предостапвляющий статичные данные для генерации и сверки ключей для Bearer токена
    /// </summary>
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer";
        public const string AUDIENCE = "MyAuthClient";
        // TODO: Найти способ генерировать ключ, например во время запуска приложения и пересоздавать по истечению некоторого промежутка времени
        public const string KEY = "secret_secretDefaultKey!";
        
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));

        //public static string generateKey()
        //{
        //    Random rnd = new Random();
        //    string strKey = "";
        //    for (int i = 0; i < 36; i++) {
        //        int value = rnd.Next();
        //        strKey += (char)value;
        //    }
        //    return Convert.ToBase64String(Encoding.ASCII.GetBytes(strKey));
        //}
    }
}
