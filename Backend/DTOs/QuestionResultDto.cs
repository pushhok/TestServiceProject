namespace TestService.DTOs
{
    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string? UserAnswerText { get; set; }
        public string? CorrectAnswerText { get; set; }
    }
}