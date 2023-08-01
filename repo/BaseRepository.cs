using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;
using WebHelloApp.models;

namespace WebHelloApp.repo
{
    /// <summary>
    /// Базовый класс для управления объектами сущностей в БД, наследемый от <see cref="IBaseRepository{T}"/>
    /// </summary>
    /// <typeparam name="T">тип объекта сущности, должен обязательно наследоваться от <see cref="BaseModel"/></typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseModel
    {
        private DatabaseService db { get; set; }
        private DbSet<T> dbSet;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BaseRepository(DatabaseService databaseService, IHttpContextAccessor httpContextAccessor)
        {
            db = databaseService;
            this.dbSet = this.db.Set<T>();
            this._httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Создает новый объект сущности
        /// </summary>
        /// <param name="model">объект сущности на создание</param>
        /// <returns>созданный объект сущности</returns>
        public async Task<T?> create(T model)
        {
            var user = _httpContextAccessor.HttpContext?.User.Identity;
            model.userCreationId = (await getUser(user?.Name))?.id;
            model.userLastModificationId = model.userCreationId;
            await dbSet.AddAsync(model);
            await db.SaveChangesAsync();
            return model;
        }

        /// <summary>
        /// Удаляет объект сущности найденный по значению первичного ключа
        /// </summary>
        /// <param name="id">значение первичного ключа в GUID формате</param>
        public async void delete(Guid id)
        {
            T? toDelete = await findById(id);
            if (toDelete == null) throw new ArgumentNullException(nameof(toDelete));
            dbSet.Remove(toDelete);
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Обнвляет имеющийся объект сущности
        /// </summary>
        /// <param name="model">объект сущности на обновление</param>
        /// <returns>обновленный объект сущности</returns>
        public async Task<T?> update(T model)
        {
            T? toUpdate = await findById(model.id);
            if (toUpdate != null)
            {
                var user = _httpContextAccessor.HttpContext?.User.Identity;
                toUpdate.dateLastModification = DateTime.Now;
                toUpdate.userLastModificationId = (await getUser(user?.Name))?.id;
                db.Entry(model).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return toUpdate;
            }
            else return null;
        }

        /// <summary>
        /// Осуществляет поиск уникального объект сущности по её первичному ключу
        /// </summary>
        /// <param name="id">значение первичного ключа в GUID формате</param>
        /// <returns>найденный объект сущности или null</returns>
        public async Task<T?> findById(Guid id) => await dbSet.AsNoTracking().FirstOrDefaultAsync(m => m.id == id);

        /// <summary>
        /// Получает объект сущности из БД согласно переданному в параметры делегату
        /// </summary>
        /// <param name="predicate">делегат для поиска объекта сущности по БД</param>
        /// <returns>объект сущности</returns>
        public IEnumerable<T> get(Func<T, bool> predicate)
        {
            return dbSet.AsNoTracking().Where(predicate);

        }

        /// <summary>
        /// Получает список объектов сущности, включающих связанные данные из дргуих таблиц
        /// по заданному делегату через .Include()
        /// </summary>
        /// <param name="includeProperties">делегат для связи с другими таблицами</param>
        /// <returns>список найденных объектов</returns>
        public IEnumerable<T> GetWithInclude(params Expression<Func<T, object>>[] includeProperties)
        {
            return Include(includeProperties).ToList();
        }

        /// <summary>
        /// Получает список объектов сущности согласно переданному в параметры делегату, включающих связанные данные из дргуих таблиц
        /// по заданному делегату через .Include()
        /// </summary>
        /// <param name="predicate">делегат для поиска объекта сущности по БД</param>
        /// <param name="includeProperties">делегат для связи с другими таблицами</param>
        /// <returns></returns>
        public IEnumerable<T> GetWithInclude(Func<T, bool> predicate,
            params Expression<Func<T, object>>[] includeProperties)
        {
            var query = Include(includeProperties);
            return query.Where(predicate).ToList();
        }

        private IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet.AsSplitQuery();
            return includeProperties
                .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        /// <summary>
        /// Метод вынесенный из UserService для доступа извне для других классов, наследуемых от базовой модели
        /// Необходим для получения объекта пользователя из БД по логину, используется для получения авторизованного пользователя из <seealso cref="HttpContent"/> запроса
        /// </summary>
        /// <param name="userAuthLogin">логин авторизованного пользователя</param>
        /// <returns>объект пользователя ИЛИ null</returns>
        public async Task<User?> getUser(string? userAuthLogin)
        {
            return userAuthLogin is null ? null : await db.Users.AsNoTracking().FirstOrDefaultAsync(m => m.login == userAuthLogin);
        }
    }
}
