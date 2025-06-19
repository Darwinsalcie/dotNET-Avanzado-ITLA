using Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.RequesDTO
{
    public class UpdateTodoRequestDto
    {
        public string Title { get; set; }

        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DueDate { get; set; }
        public Status Status { get; set; }
        public Priority? Priority { get; set; }
        public string? AdditionalData { get; set; }
    }
}
