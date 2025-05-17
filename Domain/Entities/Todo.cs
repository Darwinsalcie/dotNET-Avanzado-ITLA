
using Domain.Enums;
using Domain.Delegates;

namespace Domain.Entities
{
    public class Todo
    {
        public int Id { get;  set; }
        public string Title { get;  set; }
        public string Description { get;  set; }
        public DateTime CreatedAt { get;  set; }
        public DateTime? DueDate { get;  set; }
        public bool IsCompleted { get;  set; }
        public Status Status { get;  set; }
        public Priority? Priority { get;  set; }
        public string? AdditionalData { get;  set; }


        public Todo(string title, string description, DateTime? dueDate, string? additionalData, Status status, Priority? priority)
        {
            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            DueDate = dueDate;
            IsCompleted = false;
            Status = status;
            AdditionalData = additionalData;
            Priority = priority;
        }

        public void Update(string title, string description, DateTime? dueDate, string? additionalData, Status status, bool iscompleted, Priority? priority )
        {

            Title = title;
            Description = description;
            DueDate = dueDate;
            Status = status;
            AdditionalData = additionalData;
            IsCompleted = iscompleted;
            Priority = priority;
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
        }
    }
}
