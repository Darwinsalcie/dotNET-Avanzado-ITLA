using Application.DTOs;


namespace Application.Interfaces
{
    public interface ITodoService<T>
    {
        Task<IEnumerable<TodoDto<T>>> GetAllAsync();
        Task<TodoDto<T>?> GetByIdAsync(int id);
        Task<TodoDto<T>> CreateAsync(CreateTodoDto<T> dto);
        Task<TodoDto<T>?> UpdateAsync(int id, UpdateTodoDto<T> dto);
        Task<bool> DeleteAsync(int id);
    }
}
