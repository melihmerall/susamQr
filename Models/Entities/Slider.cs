using System.ComponentModel.DataAnnotations;

namespace susamQr.Models.Entities
{
    public class Slider : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Url]
        public string ImageUrl { get; set; }
    }
}
