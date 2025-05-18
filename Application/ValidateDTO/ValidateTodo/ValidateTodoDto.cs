using Application.DTOs.RequesDTO;
using Domain.Entities;


namespace Application.ValidateDTO.ValidateTodo
{
    public class ValidateTodoDto
    {

        public List<string> Validate(Todo dto)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(dto.Title))
                errors.Add("Title is required.");

            if (dto.DueDate.HasValue && dto.DueDate.Value.Date < DateTime.UtcNow.Date)
                errors.Add("DueDate cannot be in the past.");

            if (dto.Description?.Length > 200)
                errors.Add("Description cannot exceed 200 characters.");

            return errors;
        }

    }
}
