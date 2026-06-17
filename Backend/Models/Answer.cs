using System.ComponentModel.DataAnnotations;

namespace TestService.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        public Question? Question { get; set; }
    }
}