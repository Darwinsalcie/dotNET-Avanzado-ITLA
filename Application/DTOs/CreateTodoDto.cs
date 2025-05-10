using Domain.Enums;
using System.ComponentModel.DataAnnotations;


namespace Application.DTOs
{
    public class CreateTodoDto<T>
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get;  set; }

        public DateTime? DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public T? AdditionalData { get; set; }


    }
}
