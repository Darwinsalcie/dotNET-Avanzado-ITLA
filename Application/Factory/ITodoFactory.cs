using Application.DTOs.RequesDTO;
using Domain.Entities;


namespace Application.Factory
{
    public interface ITodoFactory
    {
        Todo Create(CreateTodoRequestDto dto);
        Todo CreateHighPriority(CreateTodoRequestDto dto);
        Todo CreateMediumPriority(CreateTodoRequestDto dto);
        Todo CreateLowPriority(CreateTodoRequestDto dto);
    }
}
