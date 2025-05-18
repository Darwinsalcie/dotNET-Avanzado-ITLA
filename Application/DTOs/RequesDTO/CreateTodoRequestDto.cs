using Domain.Enums;
using System.ComponentModel.DataAnnotations;


namespace Application.DTOs.RequesDTO
{
    public class CreateTodoRequestDto
    {

        public string Title { get; set; } 

        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Status Status { get; set; }
        public Priority? Priority { get; set; }
        public string? AdditionalData { get; set; }


    }
}
