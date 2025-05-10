using Application.DTOs;
using Domain.Entities;


namespace Application.Interfaces
{
    public interface ITodoService<T>
    {
        Task<IEnumerable<TodoDto<T>>> GetAllAsync();
        Task<TodoDto<T>?> GetByIdAsync(int id);

        Task<IEnumerable<TodoDto<T>>> GetPendingAsync();
        Task<IEnumerable<TodoDto<T>>> GetOverdueAsync();
        Task<IEnumerable<TodoDto<T>>> SearchAsync(string keyword);

        Task<TodoDto<T>> CreateAsync(CreateTodoDto<T> dto);
        Task<TodoDto<T>?> UpdateAsync(int id, UpdateTodoDto<T> dto);
        Task<bool> DeleteAsync(int id);
    }
}
