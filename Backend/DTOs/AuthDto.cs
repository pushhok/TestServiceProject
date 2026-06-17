using System.ComponentModel.DataAnnotations;

namespace TestService.DTOs
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FullName { get; set; }
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}