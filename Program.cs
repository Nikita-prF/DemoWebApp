using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebHelloApp.auth;
using WebHelloApp.service;

var builder = WebApplication.CreateBuilder();

// Добваляем авторизацию по Bearer токену
// Данные для генерации ключа предоставляет класс AuthOptions
builder.Services.AddAuthentication("Bearer").AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    }
    );
builder.Services.AddAuthorization();

builder.Services.AddControllers();
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DatabaseService>(options => options.UseSqlServer(connection));
builder.Services.AddTransient<UserService>();
// Добавляем синглтоном IHttpContextAccessor для того, чтоб инжектить в классы HttpContext и получать данные по авторизированному пользователю
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


var app = builder.Build();
app.MapControllers();
app.Run();

