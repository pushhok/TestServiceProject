using TestService.Data;
using TestService.Models;

namespace TestService.Services
{
    public interface ITestGradingService
    {
        Task<GradeResult?> GradeTestAsync(int testId, Dictionary<int, List<int>> userAnswers, AppDbContext db);
    }

    public class GradeResult
    {
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public List<QuestionResult> Details { get; set; } = new();
    }

    public class QuestionResult
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string? UserAnswerText { get; set; }
        public string? CorrectAnswerText { get; set; }
    }
}