namespace TestService.DTOs
{
    public class TestViewDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<QuestionViewDto> Questions { get; set; } = new();
    }

    public class QuestionViewDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string QuestionType { get; set; } = "single";
        public List<AnswerViewDto> Options { get; set; } = new();
    }

    public class AnswerViewDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}