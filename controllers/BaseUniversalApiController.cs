using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebHelloApp.models;
using WebHelloApp.repo;

namespace WebHelloApp.controllers
{
    /// <summary>
    /// Базовый контроллер, реализующий методы управления сущностями в БД
    /// </summary>
    /// <typeparam name="T">Параметр, принимающий тип определенной сущности</typeparam>
    [ApiController]
    [Authorize]
    public abstract class BaseUniversalApiController<T> : Controller
        where T : BaseModel
    {
        private IBaseRepository<T> repository = null!;
        public BaseUniversalApiController(DatabaseService database, IHttpContextAccessor httpContextAccessor) {
            repository = new BaseRepository<T> (database, httpContextAccessor);
        }

        /// <summary>
        /// Получает объект сущности из БД по первичному ключу id - guid'у
        /// </summary>
        /// <param name="guid">значение id ключа для поиска сущности</param>
        /// <returns>404 - если объект сущности не найден, 200 - и объект сущности в JSON, если объект найден</returns>
        [HttpGet("{guid:guid}")]
        public virtual async Task<IActionResult> getEntity(Guid guid)
        {

            var entity = await repository.findById(guid);
            return entity is null ? NotFound() : Ok(entity);
        }

        /// <summary>
        /// Создает новый объект сущности в таблице
        /// </summary>
        /// <param name="entity">Объект сущности для создания</param>
        /// <returns></returns>
        [HttpPost("create")]
        public virtual async Task<IActionResult> createEntity(T entity)
        {
            var createdEntity = await repository.create(entity);
            return Created(nameof(createdEntity), createdEntity);
        }

        /// <summary>
        /// Обновляет объект сущности
        /// </summary>
        /// <param name="entity">объект сущности на обновление</param>
        /// <returns>обновленный объект сущности</returns>
        [HttpPut("update")]
        public virtual async Task<IActionResult> updateEntity(T entity)
        {
            var updatedEntity = await repository.update(entity);
            return updatedEntity is null ? NotFound() : Ok(updatedEntity);
        }

        /// <summary>
        /// Удаляет объект сущности по первичному ключу
        /// </summary>
        /// <param name="id"></param>
        /// <returns>204 - если объект удален успешно</returns>
        [HttpDelete("delete/{id:guid}")]
        public virtual IActionResult deleteEntity(Guid id)
        {
            repository.delete(id);
            return NoContent();
        }
    }
}
