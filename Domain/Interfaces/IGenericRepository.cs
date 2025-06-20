

namespace Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id, int userId);
        Task<IEnumerable<T>> GetAllAsync(int userId);
        Task <(bool IsSucces, string Message)> AddAsync(T entity);
        Task<(bool IsSucces, string Message)> UpdateAsync(T entity);
        Task<(bool IsSucces, string Message)> DeleteAsync(int id);

        Task<(bool IsSucces, string Message)> DeleteSoftAsync(int id);

    }
}
