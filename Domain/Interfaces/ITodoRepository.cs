

using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ITodoRepository : IGenericRepository<Todo>
    {
        Task<IEnumerable<Todo>> GetByStatusAsync(int status);


    }
}
