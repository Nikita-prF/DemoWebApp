using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebHelloApp.models
{
    /// <summary>
    /// Базовая модель для всех сущностей в БД
    /// Имеет некоторые поля присущие всем сущностям и имеющие системный характер
    /// </summary>
    public class BaseModel
    {
        public BaseModel()
        {
            this.dateCreation = DateTime.Now;
            this.dateLastModification = this.dateCreation;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { get; set; }
        public DateTime dateCreation { get; set; }
        public DateTime? dateLastModification { get; set; } = null;
        public Guid? userCreationId { get; set; } = null;
        public Guid? userLastModificationId { get; set; } = null;
    }
}
