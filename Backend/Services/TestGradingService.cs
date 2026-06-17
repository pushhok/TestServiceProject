using Microsoft.EntityFrameworkCore;
using TestService.Data;
using TestService.Models;

namespace TestService.Services
{
    public class TestGradingService : ITestGradingService
    {
        public async Task<GradeResult?> GradeTestAsync(int testId, Dictionary<int, List<int>> userAnswers, AppDbContext db)
        {
            var test = await db.Tests
                .Include(t => t.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(t => t.Id == testId);

            if (test == null)
                return null;

            var result = new GradeResult
            {
                TotalQuestions = test.Questions.Count
            };

            foreach (var question in test.Questions)
            {
                var correctAnswers = question.Answers.Where(a => a.IsCorrect).Select(a => a.Id).ToList();
                userAnswers.TryGetValue(question.Id, out List<int> userAnswerIds);
                var userAnswerIdsList = userAnswerIds ?? new List<int>();

                bool isCorrect;
                
                if (question.QuestionType == "multiple")
                {
                    isCorrect = correctAnswers.Count == userAnswerIdsList.Count && 
                               correctAnswers.All(id => userAnswerIdsList.Contains(id));
                }
                else
                {
                    isCorrect = userAnswerIdsList.Count == 1 && 
                               correctAnswers.Contains(userAnswerIdsList[0]);
                }

                if (isCorrect)
                    result.Score++;

                result.Details.Add(new QuestionResult
                {
                    QuestionId = question.Id,
                    QuestionText = question.Text,
                    IsCorrect = isCorrect,
                    UserAnswerText = string.Join(", ", question.Answers
                        .Where(a => userAnswerIdsList.Contains(a.Id))
                        .Select(a => a.Text)),
                    CorrectAnswerText = string.Join(", ", correctAnswers
                        .Select(id => question.Answers.FirstOrDefault(a => a.Id == id)?.Text)
                        .Where(t => t != null))
                });
            }

            return result;
        }
    }
}