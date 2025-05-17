
using Domain.Enums;
using Domain.Delegates;

namespace Domain.Entities
{
    public class Todo
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get;  private set; }
        public DateTime? DueDate { get;  private set; }
        public bool IsCompleted { get;  private set; }
        public Status Status { get;  private set; }
        public Priority? Priority { get;  private set; }
        public string? AdditionalData { get;  private set; }


        // Resolver (Bloquea get tareas pasadas)
        EntityValidator<Todo> validate = todo =>
            !string.IsNullOrWhiteSpace(todo.Title)
            && (!todo.DueDate.HasValue || todo.DueDate > DateTime.UtcNow);

        public Todo(string title, string? description, DateTime? dueDate, string? additionalData, Status status, Priority? priority)
        {
   

            Title = title;
            Description = description;
            CreatedAt = DateTime.UtcNow;
            DueDate = dueDate;
            IsCompleted = false;
            Status = status;
            AdditionalData = additionalData;
            Priority = priority;

            if (!validate(this))
                throw new ArgumentException("Invalid Todo entity");
        }

        public void Update(string title, string? description, DateTime? dueDate, string? additionalData, Status status, bool iscompleted, Priority? priority )
        {


            Title = title;
            Description = description;
            DueDate = dueDate;
            Status = status;
            AdditionalData = additionalData;
            IsCompleted = iscompleted;
            Priority = priority;


            if (!validate(this))
                throw new ArgumentException("Invalid Todo entity");
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
        }
    }
}
