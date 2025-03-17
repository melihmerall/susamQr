using System.ComponentModel.DataAnnotations;

namespace susamQr.Models.Entities
{
    public class Setting : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(2000)] // Ayar Değeri uzun olabilir
        public string Value { get; set; }
    }
}
