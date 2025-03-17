using System.ComponentModel.DataAnnotations;

namespace susamQr.Models.Entities
{
    public class Category : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; }

        public bool Status { get; set; }
		[Url]
		public string? ImageUrl { get; set; }
	}
}

