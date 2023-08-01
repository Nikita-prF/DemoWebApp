using System.Linq.Expressions;
using WebHelloApp.models;

namespace WebHelloApp.repo;

/// <summary>
/// Интерфейс для реализации базовых методов управления объектами сущностей в БД
/// </summary>
/// <typeparam name="T">Типы сущностей (обязательно должны насоедоваться от <see cref="BaseModel"/>)</typeparam>
public interface IBaseRepository<T> where T : BaseModel
{
    /// <summary>
    /// Получает объект сущности из БД согласно переданному в параметры делегату
    /// </summary>
    /// <param name="predicate">делегат для поиска объекта сущности по БД</param>
    /// <returns>объект сущности</returns>
    IEnumerable<T> get(Func<T, bool> predicate);

    /// <summary>
    /// Получает список объектов сущности, включающих связанные данные из дргуих таблиц
    /// по заданному делегату через .Include()
    /// </summary>
    /// <param name="includeProperties">делегат для связи с другими таблицами</param>
    /// <returns>список найденных объектов</returns>
    IEnumerable<T> GetWithInclude(params Expression<Func<T, object>>[] includeProperties);

    /// <summary>
    /// Получает список объектов сущности согласно переданному в параметры делегату, включающих связанные данные из дргуих таблиц
    /// по заданному делегату через .Include()
    /// </summary>
    /// <param name="predicate">делегат для поиска объекта сущности по БД</param>
    /// <param name="includeProperties">делегат для связи с другими таблицами</param>
    /// <returns></returns>
    IEnumerable<T> GetWithInclude(Func<T, bool> predicate, params Expression<Func<T, object>>[] includeProperties);

    /// <summary>
    /// Осуществляет поиск уникального объект сущности по её перчиному ключу
    /// </summary>
    /// <param name="id">значение первичного ключа в GUID формате</param>
    /// <returns>найденный объект сущности или null</returns>
    Task<T?> findById(Guid id);

    /// <summary>
    /// Создает новый объект сущности
    /// </summary>
    /// <param name="model">объект сущности на создание</param>
    /// <returns>созданный объект сущности</returns>
    Task<T?> create(T model);


    /// <summary>
    /// Обнвляет имеющийся объект сущности
    /// </summary>
    /// <param name="model">объект сущности на обновление</param>
    /// <returns>обновленный объект сущности</returns>
    Task<T?> update(T model);


    /// <summary>
    /// Удаляет объект сущности найденный по значению первичного ключа
    /// </summary>
    /// <param name="id">значение первичного ключа в GUID формате</param>
    void delete(Guid id);
}
