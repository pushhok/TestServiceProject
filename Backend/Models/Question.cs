using System.ComponentModel.DataAnnotations;

namespace TestService.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public string QuestionType { get; set; } = "single";

        public int TimeLimit { get; set; } = 0;

        public int TestId { get; set; }
        public Test? Test { get; set; }

        public List<Answer> Answers { get; set; } = new();
    }
}
