

using System.ComponentModel.DataAnnotations;

namespace WebHelloApp.models
{
    /// <summary>
    /// Класс для сущностей типа Организация в БД
    /// </summary>
    public class Unit : BaseModel
    {
        [Required]
        public string? name { get; set; } = null;
        List<Unit> users { get; set; } = new();

    }
}
