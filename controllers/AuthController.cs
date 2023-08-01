using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebHelloApp.auth;
using WebHelloApp.service;

namespace WebHelloApp.controllers
{
    /// <summary>
    /// Контроллер авторизации и аутентификации, наследуемый от <see cref="Controller"/>
    /// </summary>
    [ApiController]
    [Route("/auth")]
    public class AuthController : Controller
    {

        private UserService _userService;
        public AuthController(UserService userService) => _userService = userService;

        /// <summary>
        /// Контроллер для авторизации по логину
        /// </summary>
        /// <param name="login">логин пользователя из БД</param>
        /// <returns>JSON с Bearer токеном, используемым для дальнейших запросов к контроллерам</returns>
        [HttpGet("{login}")]
        public IActionResult auth(string login) {
            
            if (string.IsNullOrEmpty(login)) return NotFound(login);
           
            if (!_userService.isUserExist(login)) return NotFound(login);

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, login) };

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Ok(new
            {
                access_token = encodedJwt,
                username = login
            });
        }

        /// <summary>
        /// Контроллер для выхода из сессии. Удаляет данные авторизации пользвателя.
        /// </summary>
        /// <returns>200 OK - если сессия успешно удалена, 400 - если данного пользователя не существует или он не авторизован</returns>
        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> logout()
        {
            var user = HttpContext.User.Identity;
            if (user is not null && user.IsAuthenticated)
            {
                await HttpContext.SignOutAsync();
                return Ok();
            }
            else return BadRequest();
        }
    }
}
