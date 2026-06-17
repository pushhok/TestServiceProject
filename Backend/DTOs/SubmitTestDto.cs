namespace TestService.DTOs
{
    public class SubmitTestDto
    {
        public Dictionary<int, List<int>> Answers { get; set; } = new();
    }
}