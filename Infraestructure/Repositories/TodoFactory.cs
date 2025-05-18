using Application.DTOs.RequesDTO;
using Application.ValidateDTO.ValidateTodo;
using Domain.Entities;
using Domain.Enums;
using Application.Factory;

namespace Infraestructure.Repositories
{
    public class TodoFactory : ITodoFactory
    {

        private readonly CreateTodoDtoValidator _validator;
        public TodoFactory(CreateTodoDtoValidator validator)
        {
            _validator = validator;
        }


        public Todo Create(CreateTodoRequestDto dto)
        {
            _validator.Validate(dto);
            return Build(dto, dto.Priority ?? Priority.Medium, defaultDays: 7);
        }


        public Todo CreateHighPriority(CreateTodoRequestDto dto)
        {
            _validator.Validate(dto);
            return Build(dto, Priority.High, defaultDays: 2);
        }


        public Todo CreateLowPriority(CreateTodoRequestDto dto)
        {
            _validator.Validate(dto);
            return Build(dto, Priority.Low, defaultDays: 30);
        }

 

        public Todo CreateMediumPriority(CreateTodoRequestDto dto)
        {
            _validator.Validate(dto);
            return Build(dto, Priority.Medium, defaultDays: 7);
        }


        private Todo Build(CreateTodoRequestDto dto, Priority priority, int defaultDays)
        {
            DateTime? due = dto.DueDate.HasValue
                ? dto.DueDate.Value
                : DateTime.UtcNow.AddDays(defaultDays);

            return new Todo(
                title: dto.Title,
                description: dto.Description,
                dueDate: due,
                additionalData: dto.AdditionalData,
                status: dto.Status,
                priority: priority
            );
        }
    }
}
