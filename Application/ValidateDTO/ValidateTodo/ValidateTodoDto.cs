using Application.DTOs.RequesDTO;
using Domain.Entities;
using Domain.Enums;


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

            if (dto.AdditionalData?.Length > 500)
                errors.Add("AdditionalData cannot exceed 500 characters.");

            //validación de Priority
            if (dto.Priority.HasValue && !Enum.IsDefined(typeof(Priority), dto.Priority.Value))
                errors.Add("Invalid Priority value.");
            //validación de Status
            //Se supone que Status es notnull, poor eso no tiene el HasValue
            if (!Enum.IsDefined(typeof(Status), dto.Status))
                errors.Add("Invalid Status value.");

            return errors;
        }


    }
}
