using Application.DTOs.RequesDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ValidateDTO.ValidateTodo
{
    public class CreateTodoDtoValidator
    {
        /// <summary>
        /// Lanza ArgumentException si el DTO es inválido.
        /// </summary>
        public void Validate(CreateTodoRequestDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Title))
                errors.Add("El título es obligatorio.");

            if (dto.DueDate.HasValue && dto.DueDate.Value <= DateTime.UtcNow)
                errors.Add("La fecha de vencimiento debe ser futura.");

            if (errors.Any())
                throw new ArgumentException(string.Join(" | ", errors));
        }
    }
}
