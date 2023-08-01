using WebHelloApp.models;
using WebHelloApp.repo;

namespace WebHelloApp.service
{
    /// <summary>
    /// Класс, описывающий дополнительную логику по работе с сущностью типа <seealso cref="User"/>
    /// Наследуется от <seealso cref="BaseRepository{T}"/> и потому имеет все базовые методы по работе с сущностями
    /// </summary>
    public class UserService : BaseRepository<User>
    {
        private DatabaseService db;

        public UserService(DatabaseService database, IHttpContextAccessor httpContextAccessor) : base(database, httpContextAccessor) {
            db = database;
        }
        /// <summary>
        /// Проверяет, существует ли пользователь с указанным логином в БД
        /// </summary>
        /// <param name="login">логин пользователя</param>
        /// <returns><code>true</code> - если пользователь существует; <code>false</code> - если нет</returns>
        public bool isUserExist(string login) => db.Users.Count(u => u.login == login) > 0;
    }
}
