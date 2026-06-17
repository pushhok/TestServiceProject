using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TestService.Data;
using TestService.DTOs;
using TestService.Models;
using TestService.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=testservice.db";

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(connectionString));

builder.Services.AddScoped<ITestGradingService, TestGradingService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy => policy
        .WithOrigins("http://localhost:5173")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

var jwtSecret = builder.Configuration["Jwt:Secret"] 
    ?? Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? "your-256-bit-secret-key-here-must-be-at-least-32-chars-long";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "TestService",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "TestServiceClient",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors("AllowVueApp");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/auth/register", async (RegisterDto dto, IAuthService authService) =>
{
    var (user, error) = await authService.RegisterAsync(dto);
    if (error != null)
        return Results.BadRequest(new { message = error });

    var token = authService.GenerateToken(user);
    return Results.Ok(new AuthResponseDto
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        Token = token
    });
});

app.MapPost("/api/auth/login", async (LoginDto dto, IAuthService authService) =>
{
    var (user, error) = await authService.LoginAsync(dto);
    if (error != null)
        return Results.Json(new { message = error }, statusCode: 401);

    var token = authService.GenerateToken(user);
    return Results.Ok(new AuthResponseDto
    {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        Token = token
    });
});

app.MapGet("/api/auth/me", (ClaimsPrincipal user) =>
{
    var id = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    var email = user.FindFirstValue(ClaimTypes.Email) ?? "";
    var name = user.FindFirstValue(ClaimTypes.Name) ?? "";

    return Results.Ok(new { id, email, fullName = name });
}).RequireAuthorization();

app.MapGet("/api/tests", async (AppDbContext db) =>
{
    var tests = await db.Tests
        .Include(t => t.Questions)
        .Select(t => new
        {
            t.Id,
            t.Title,
            t.Description,
            t.CreatedAt,
            t.ShowCorrectAnswers,
            t.TotalTimeLimit,
            QuestionsCount = t.Questions.Count,
            CreatorName = t.Creator != null ? t.Creator.FullName : "Аноним"
        })
        .ToListAsync();
    return Results.Ok(tests);
});

app.MapGet("/api/tests/{id:int}", async (int id, AppDbContext db) =>
{
    var test = await db.Tests
        .Include(t => t.Questions)
        .ThenInclude(q => q.Answers)
        .FirstOrDefaultAsync(t => t.Id == id);

    if (test == null) return Results.NotFound(new { message = "Тест не найден" });

    return Results.Ok(new
    {
        test.Id,
        test.Title,
        test.Description,
        test.ShowCorrectAnswers,
        test.TotalTimeLimit,
        Questions = test.Questions.Select(q => new
        {
            q.Id,
            q.Text,
            q.QuestionType,
            q.TimeLimit,
            Options = q.Answers.Select(a => new { a.Id, a.Text, a.IsCorrect }).ToList()
        }).ToList()
    });
});

app.MapPost("/api/tests", async (
    CreateTestDto dto, 
    AppDbContext db, 
    ClaimsPrincipal user) =>
{
    if (user.Identity?.IsAuthenticated != true)
        return Results.Json(new { message = "Необходима авторизация" }, statusCode: 401);

    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    try
    {
        for (int i = 0; i < dto.Questions.Count; i++)
        {
            var question = dto.Questions[i];
            if (!question.Answers.Any(a => a.IsCorrect))
            {
                return Results.BadRequest(new 
                { 
                    message = $"В вопросе {i + 1} не указан правильный ответ." 
                });
            }
        }

        var test = new Test
        {
            Title = dto.Title,
            Description = dto.Description,
            ShowCorrectAnswers = dto.ShowCorrectAnswers,
            TotalTimeLimit = dto.TotalTimeLimit,
            CreatedAt = DateTime.UtcNow,
            CreatorId = userId,
            Questions = new List<Question>()
        };

        foreach (var qDto in dto.Questions)
        {
            var question = new Question
            {
                Text = qDto.Text,
                QuestionType = qDto.QuestionType,
                TimeLimit = qDto.TimeLimit,
                Answers = new List<Answer>()
            };

            foreach (var aDto in qDto.Answers)
            {
                question.Answers.Add(new Answer { Text = aDto.Text, IsCorrect = aDto.IsCorrect });
            }
            test.Questions.Add(question);
        }

        db.Tests.Add(test);
        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            test.Id,
            test.Title,
            test.Description,
            test.ShowCorrectAnswers,
            test.TotalTimeLimit,
            test.CreatedAt,
            QuestionsCount = test.Questions.Count
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating test: {ex.Message}");
        return Results.Problem($"Ошибка при создании теста: {ex.Message}");
    }
}).RequireAuthorization();

app.MapPut("/api/tests/{id:int}", async (
    int id, 
    CreateTestDto dto, 
    AppDbContext db, 
    ClaimsPrincipal user) =>
{
    if (user.Identity?.IsAuthenticated != true)
        return Results.Json(new { message = "Необходима авторизация" }, statusCode: 401);

    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    try
    {
        var test = await db.Tests
            .Include(t => t.Questions)
            .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (test == null) 
            return Results.NotFound(new { message = "Тест не найден" });

        if (test.CreatorId != userId)
            return Results.Json(new { message = "У вас нет прав на редактирование этого теста" }, statusCode: 403);

        for (int i = 0; i < dto.Questions.Count; i++)
        {
            var question = dto.Questions[i];
            if (!question.Answers.Any(a => a.IsCorrect))
            {
                return Results.BadRequest(new 
                { 
                    message = $"В вопросе {i + 1} не указан правильный ответ." 
                });
            }
        }

        test.Title = dto.Title;
        test.Description = dto.Description;
        test.ShowCorrectAnswers = dto.ShowCorrectAnswers;
        test.TotalTimeLimit = dto.TotalTimeLimit;

        db.Questions.RemoveRange(test.Questions);
        
        var newQuestions = new List<Question>();
        foreach (var qDto in dto.Questions)
        {
            var question = new Question
            {
                Text = qDto.Text,
                QuestionType = qDto.QuestionType,
                TimeLimit = qDto.TimeLimit,
                Answers = new List<Answer>()
            };

            foreach (var aDto in qDto.Answers)
            {
                question.Answers.Add(new Answer { Text = aDto.Text, IsCorrect = aDto.IsCorrect });
            }
            newQuestions.Add(question);
        }

        test.Questions = newQuestions;
        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            test.Id,
            test.Title,
            test.Description,
            test.ShowCorrectAnswers,
            test.TotalTimeLimit,
            QuestionsCount = test.Questions.Count
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error updating test: {ex.Message}");
        return Results.Problem($"Ошибка при обновлении теста: {ex.Message}");
    }
}).RequireAuthorization();

app.MapDelete("/api/tests/{id:int}", async (
    int id, 
    AppDbContext db, 
    ClaimsPrincipal user) =>
{
    if (user.Identity?.IsAuthenticated != true)
        return Results.Json(new { message = "Необходима авторизация" }, statusCode: 401);

    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    try
    {
        var test = await db.Tests.FindAsync(id);
        if (test == null) 
            return Results.NotFound(new { message = "Тест не найден" });

        if (test.CreatorId != userId)
            return Results.Json(new { message = "У вас нет прав на удаление этого теста" }, statusCode: 403);

        db.Tests.Remove(test);
        await db.SaveChangesAsync();

        return Results.Ok(new { message = "Тест удалён" });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error deleting test: {ex.Message}");
        return Results.Problem($"Ошибка при удалении теста: {ex.Message}");
    }
}).RequireAuthorization();

app.MapPost("/api/tests/{id:int}/submit", async (
    int id, 
    SubmitTestDto submission, 
    ITestGradingService gradingService, 
    AppDbContext db,
    ClaimsPrincipal user) =>
{
    int? userId = null;
    if (user.Identity?.IsAuthenticated == true)
    {
        userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }

    try
    {
        var result = await gradingService.GradeTestAsync(id, submission.Answers, db);
        if (result == null) 
            return Results.NotFound(new { message = "Тест не найден" });

        var test = await db.Tests.FindAsync(id);
        var showCorrectAnswers = test?.ShowCorrectAnswers ?? false;

        var testResult = new TestResult
        {
            TestId = id,
            UserId = userId,
            Score = result.Score,
            TotalQuestions = result.TotalQuestions,
            CompletedAt = DateTime.UtcNow,
            Details = showCorrectAnswers ? JsonSerializer.Serialize(result.Details) : null
        };

        db.TestResults.Add(testResult);
        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            result.Score,
            result.TotalQuestions,
            Percentage = Math.Round((double)result.Score / result.TotalQuestions * 100, 2),
            ShowCorrectAnswers = showCorrectAnswers,
            Details = showCorrectAnswers ? result.Details : null
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error submitting test: {ex.Message}");
        return Results.Problem($"Ошибка при отправке теста: {ex.Message}");
    }
});

app.MapGet("/api/results", async (AppDbContext db, ClaimsPrincipal user) =>
{
    var query = db.TestResults
        .Include(r => r.Test)
        .OrderByDescending(r => r.CompletedAt)
        .AsQueryable();

    if (user.Identity?.IsAuthenticated == true)
    {
        var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        query = query.Where(r => r.UserId == userId);
    }

    var results = await query.ToListAsync();

    var response = results.Select(r => 
    {
        List<QuestionResultDto>? details = null;
        if (r.Details != null)
        {
            try
            {
                details = JsonSerializer.Deserialize<List<QuestionResultDto>>(r.Details);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing details: {ex.Message}");
            }
        }

        return new
        {
            r.Id,
            TestName = r.Test != null ? r.Test.Title : "Неизвестный тест",
            r.Score,
            r.TotalQuestions,
            r.CompletedAt,
            Details = details
        };
    });

    return Results.Ok(response);
});

app.MapGet("/api/my-tests", async (AppDbContext db, ClaimsPrincipal user) =>
{
    if (user.Identity?.IsAuthenticated != true)
        return Results.Json(new { message = "Необходима авторизация" }, statusCode: 401);

    var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    var tests = await db.Tests
        .Where(t => t.CreatorId == userId)
        .Include(t => t.Questions)
        .Select(t => new
        {
            t.Id,
            t.Title,
            t.Description,
            t.CreatedAt,
            t.ShowCorrectAnswers,
            t.TotalTimeLimit,
            QuestionsCount = t.Questions.Count
        })
        .ToListAsync();

    return Results.Ok(tests);
}).RequireAuthorization();

app.MapFallbackToFile("index.html");

app.Run();