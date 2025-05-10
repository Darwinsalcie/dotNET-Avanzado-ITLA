using Application.DTOs;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Entities;
using Domain.Delegates;
using Domain.Exceptions;

namespace Application.Services
{
    public class TodoService<T> : ITodoService<T>
    {
        private readonly ITodoRepository<T> _repository;
        private readonly TodoValidator<Todo<T>> _validator;
        private readonly Action<Todo<T>> _notify;
        private readonly Func<Todo<T>, int> _daysRemaining;

        public TodoService(
            ITodoRepository<T> repository,
            TodoValidator<Todo<T>> validator,
            Action<Todo<T>> notify,
            Func<Todo<T>, int> daysRemaining)
        {
            _repository = repository;
            _validator = validator;
            _notify = notify;
            _daysRemaining = daysRemaining;
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
            
            if(!_validator(entity))
                throw new DomainException("Validación fallida: Title, Status o DueDate inválidos.");
            
            await _repository.AddAsync(entity);
            _notify(entity);
            var days = _daysRemaining(entity);
            return MapToDto(entity);
        }

        public async Task<TodoDto<T>?> UpdateAsync(int id, UpdateTodoDto<T> dto)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo == null)
                throw new DomainException("Todo not found.");

            todo.Update(dto.Title, dto.Description, dto.DueDate, dto.AdditionalData, dto.Status, dto.IsCompleted);

            if (!_validator(todo))
                throw new DomainException("Validation failed in Update.");

            await _repository.UpdateAsync(todo);
            return MapToDto(todo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var todo = await _repository.GetByIdAsync(id);
            if (todo == null)
                return false;

            await _repository.DeleteAsync(id);
            _notify(todo);
            return true;
        }
        public async Task<IEnumerable<TodoDto<T>>> GetPendingAsync()
        {
            var todos = await _repository.GetAllAsync();
            return todos.Where(t => !t.IsCompleted).Select(MapToDto);
        }

        public async Task<IEnumerable<TodoDto<T>>> GetOverdueAsync()
        {
            var todos = await _repository.GetAllAsync();
            return todos
                .Where(t => t.DueDate.HasValue && t.DueDate < DateTime.UtcNow)
                .Select(MapToDto);
        }

        public async Task<IEnumerable<TodoDto<T>>> SearchAsync(string keyword)
        {
            var todos = await _repository.GetAllAsync();
            return todos
                .Where(t => t.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .Select(MapToDto);
        }


        private TodoDto<T> MapToDto(Todo<T> t) => new TodoDto<T>
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            CreatedAt = t.CreatedAt,
            DueDate = t.DueDate,
            IsCompleted = t.IsCompleted,
            Status = t.Status,
            AdditionalData = t.AdditionalData,
            DaysRemaining = _daysRemaining(t)
        };
    }

}
