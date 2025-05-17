using Domain.Enums;


namespace Application.DTOs.Response
{
    public class TodoResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? Status { get; set; }
        public string? AdditionalData { get; set; }
        public DateTime? DaysRemaining { get; set; }

    }
}
