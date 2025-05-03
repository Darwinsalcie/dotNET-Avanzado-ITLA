
using Domain.Enums;

namespace Domain.Entities
{
    public class Todo<T>
    {
        public int Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }
        public DateTime? DueDate { get; private set; }
        public bool IsCompleted { get; private set; }
        public string Status { get; private set; } = string.Empty;
        public T? AdditionalData { get; private set; }


        private Todo() { }

        public Todo(string title, string description, DateTime? dueDate, T? additionalData, string status)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new Exceptions.DomainException("Title cannot be empty.");

            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            DueDate = dueDate;
            IsCompleted = false;
            Status = status;
            AdditionalData = additionalData;
        }

        public void Update(string title, string description, DateTime? dueDate, T? additionalData, string status, bool iscompleted )
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new Exceptions.DomainException("Title cannot be empty.");

            if (string.IsNullOrWhiteSpace(status))
                throw new Exceptions.DomainException("Status cannot be empty.");

            Title = title;
            Description = description;
            DueDate = dueDate;
            Status = status;
            AdditionalData = additionalData;
            IsCompleted = iscompleted;
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
        }
    }
}
