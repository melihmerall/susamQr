using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace susamQr.Models.Entities
{
    public class User : BaseEntity
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(191)] // Adjust for MySQL
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string PasswordHash { get; set; }

        public bool IsAdmin { get; set; } // Kullanıcının admin olup olmadığını belirlemek için

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
