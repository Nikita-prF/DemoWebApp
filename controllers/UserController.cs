using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebHelloApp.models;
using WebHelloApp.repo;
using WebHelloApp.service;

namespace WebHelloApp.controllers
{
    /// <summary>
    /// Контроллер, инициализирущий основной функционал управления объеками сущности типа <see cref="User"/> и реализующий дополнительные функции
    /// Наследуется от <see cref="BaseUniversalApiController{T}"/>, поэтому имеет все базовые методы
    /// </summary>
    [Route("/api/users")]
    public class UserController : BaseUniversalApiController<User>
    {
        private readonly ILogger<UserController> _logger;
        private IBaseRepository<User> _repository;
        private UserService _userService;
        public UserController(ILogger<UserController> logger, DatabaseService database, UserService userService, IHttpContextAccessor httpContextAccessor)
            : base(database, httpContextAccessor) {
            _userService = userService;
            _logger = logger;
            _repository = new BaseRepository<User>(database, httpContextAccessor);
       
        }


        /// <summary>
        /// Отладочный контроллер, для создания нового пользователя, доступный неавторизованным пользователям
        /// </summary>
        /// <param name="user">Объект нового пользователя на создание</param>
        /// <returns>201 - и объект <see cref="User"/> ИЛИ 503 - в случае ошибки</returns>
        [HttpPost("initNewUser")]
        [AllowAnonymous]
        public async Task<IActionResult> initNewUser(User user)
        {
            /*
             * Гененриурем логин и инкрементим дополнительное число к строке логина,
             * в случае, если пользователь с таким логином уже пристуствует в БД
             */
            var newUser = await createUser(user);
            if (newUser == null) return Problem();
            return Created(nameof(newUser), newUser);
        }


        [HttpPost("create")]
        public override async Task<IActionResult> createEntity(User entity)
        {
            var user = await createUser(entity);
            if (user == null) return Problem();
            return Created(nameof(user), user);
        }

        private async Task<User?> createUser(User user)
        {
            long userLoginCounter = 0;
            user.login = user.generateLogin();
            string userLoginDefault = user.login;
            while (_userService.isUserExist(user.login))
            {
                user.login = userLoginDefault + userLoginCounter;
                userLoginCounter++;
            }
            return await _userService.create(user);
        }

    }
}
