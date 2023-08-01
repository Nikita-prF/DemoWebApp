using Microsoft.IdentityModel.Tokens;
using Iuliia;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebHelloApp.models
{
    /// <summary>
    /// Класс для сущностей типа Пользователь в БД
    /// </summary>
    [Index("login", IsUnique = true)]
    public class User : BaseModel
    {
        public string? login { get; set; }
        [Required]
        [NotNull]
        [RegularExpression("^[a-zA-Zа-яёА-ЯЁ]+\\s[a-zA-Zа-яёА-ЯЁ]+(\\s[a-zA-Zа-яёА-ЯЁ]+)?$", ErrorMessage = "Данные по ФИО не прошли проверку на валидность")]
        public string? name { get; set; }
        public Guid? unitId { get; set; } = null;
        public Unit? unit { get; set; } = null;

        public User() { }

        public User(string name)
        {
            if (!name.IsNullOrEmpty()) this.name = name;
            else throw new ArgumentNullException("Имя пользователя не может быть пустым или null");
        }

        /// <summary>
        /// Гненерирует логин по ФИО из свойства name. Следует учитывать,
        /// что в name приходит ФИО с формы на UI,
        /// где фамилия, имя и отчество располгаются всегда в одном порядке и обязательны к заполнению (кроме отчества).
        /// ! Не проверяет на наличие пользователей с таким логином !
        /// ! Использует IuliiaTranslator для транслитерации !
        /// </summary>
        /// <returns>сгенерированный логин для пользователя</returns>
        public string generateLogin()
        {
            string login = string.Empty;
            if (name is not null)
            {
                string nameForLogin = name;

                nameForLogin = IuliiaTranslator.Translate(nameForLogin, Schemas.Mosmetro);
                string[] nameParts = nameForLogin.ToLower().Split(' ');
                login = $"{nameParts[1][0]}.";
                if (nameParts.Length > 2) login += $"{nameParts[2][0]}.";
                login += $"{nameParts[0]}";
            }
            return login;
        }



    }
}
