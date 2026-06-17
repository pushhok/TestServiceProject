namespace TestService.DTOs
{
    public class CreateTestDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool ShowCorrectAnswers { get; set; } = true;
        public int TotalTimeLimit { get; set; } = 0;
        public List<CreateQuestionDto> Questions { get; set; } = new();
    }

    public class CreateQuestionDto
    {
        public string Text { get; set; } = string.Empty;
        public string QuestionType { get; set; } = "single";
        public int TimeLimit { get; set; } = 0;
        public List<CreateAnswerDto> Answers { get; set; } = new();
    }

    public class CreateAnswerDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}