
using Domain.Enums;
using Domain.Delegates;

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



        private static readonly TodoValidator<Todo<T>> _validate = todo =>
        !string.IsNullOrWhiteSpace(todo.Title) 
        && !string.IsNullOrWhiteSpace(todo.Status)
        && (!todo.DueDate.HasValue || todo.DueDate > DateTime.UtcNow);

        

        public Todo(string title, string description, DateTime? dueDate, T? additionalData, string status)
        {
            if (!_validate(this))
                throw new Exceptions.DomainException("Validación fallida: Title, Status o DueDate inválidos.");

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
            if (!_validate(this))
                throw new Exceptions.DomainException("Validación fallida al actualizar entidad.");


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
