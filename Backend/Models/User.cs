using System.ComponentModel.DataAnnotations;

namespace TestService.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Test> CreatedTests { get; set; } = new();
        public List<TestResult> TestResults { get; set; } = new();
    }
}