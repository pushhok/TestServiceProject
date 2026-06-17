using System.ComponentModel.DataAnnotations;

namespace TestService.Models
{
    public class Test
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CreatorId { get; set; }
        public User? Creator { get; set; }

        public bool ShowCorrectAnswers { get; set; } = true;

        public int TotalTimeLimit { get; set; } = 0;

        public List<Question> Questions { get; set; } = new();
    }
}