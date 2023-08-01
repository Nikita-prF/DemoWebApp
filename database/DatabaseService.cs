using Microsoft.EntityFrameworkCore;
using WebHelloApp.models;

/// <summary>
/// Класс наследуемый от <see cref="DbContext"/>, реализующий объекты - экземпляры сессий подключения к БД,
/// через которые и производятся операции с базой
/// </summary>
public class DatabaseService : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Unit> Units { get; set; } = null!;
    public DatabaseService(DbContextOptions<DatabaseService> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}