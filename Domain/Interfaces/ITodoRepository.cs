

using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ITodoRepository<T>
    {
        Task<IEnumerable<Todo<T>>> GetAllAsync();
        Task<Todo<T>?> GetByIdAsync(int id);
        Task AddAsync(Todo<T> todo);
        Task UpdateAsync(Todo<T> todo);
        Task DeleteAsync(int id);
    }
}
