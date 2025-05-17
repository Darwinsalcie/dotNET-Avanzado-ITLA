using Domain.Enums;


namespace Application.DTOs.Response
{
    public class TodoResponseDTO
    {
        public int Id { get;  set; }
        public string Title { get;  set; }
        public string? Description { get;  set; }
        public DateTime CreatedAt { get;  set; }
        public DateTime? DueDate { get;  set; }
        public bool IsCompleted { get;  set; }
        public Status Status { get;  set; }
        public Priority? Priority { get;  set; }
        public string? AdditionalData { get;  set; }
        public int? DaysRemaining { get;  set; }


    }
}
