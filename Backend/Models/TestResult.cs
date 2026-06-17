using System.ComponentModel.DataAnnotations;

namespace TestService.Models
{
    public class TestResult
    {
        public int Id { get; set; }

        public int TestId { get; set; }
        public Test? Test { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        public string? Answers { get; set; }
        public string? Details { get; set; }
    }
}