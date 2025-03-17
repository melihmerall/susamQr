using System.ComponentModel.DataAnnotations;

namespace susamQr.Models.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Range(0.01, 10000)]
        public decimal? Price { get; set; }

        [Range(0.01, 10000)]
        public decimal? OldPrice { get; set; }

        [StringLength(700)]
        public string Description { get; set; }

        public bool Status { get; set; }
        public bool IsHighlight { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
