using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class TodoService<T> : ITodoService<T>
    {
        private readonly ITodoRepository<T> _repository;

        public TodoService(ITodoRepository<T> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TodoDto<T>>> GetAllAsync()
        {
            var todos = await _repository.GetAllAsync();
            return todos.Select(t => MapToDto(t));
        }

        public async Task<TodoDto<T>?> GetByIdAsync(int id)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo == null)
                return null;
            return MapToDto(todo);
        }

        public async Task<TodoDto<T>> CreateAsync(CreateTodoDto<T> dto)
        {
            var entity = new Todo<T>(dto.Title, dto.Description, dto.DueDate, dto.AdditionalData, dto.Status);
            await _repository.AddAsync(entity);
            return MapToDto(entity);
        }

        public async Task<TodoDto<T>?> UpdateAsync(int id, UpdateTodoDto<T> dto)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo == null)
                return null;

            todo.Update(dto.Title, dto.Description, dto.DueDate, dto.AdditionalData, dto.Status, dto.IsCompleted);
            await _repository.UpdateAsync(todo);
            return MapToDto(todo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo == null)
                return false;

            await _repository.DeleteAsync(id);
            return true;
        }

        private static TodoDto<T> MapToDto(Todo<T> t) => new TodoDto<T>
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            IsCompleted = t.IsCompleted,
            Status = t.Status,
            AdditionalData = t.AdditionalData
        };
    }

}
