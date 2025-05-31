
using Domain.Entities;
using System.Linq.Expressions;

namespace Domain.Interfaces
{
    public interface ITodoRepository: IGenericRepository<Todo>
    {
        Task<IEnumerable<Todo>> GetByStatusAsync(int status);
        Task<IEnumerable<Todo>> GetByPriorityAsync(int priority);
        Task<IEnumerable<Todo>> GetByTitleAsync(string title);
        Task<IEnumerable<Todo>> filterTodoAsync(int? status, int? priority, string? title, DateTime? dueDate);

        Task<double> ContarTareasCompletadasAsync();
        Task<double> ContarTareasPendientesAsync();


    }
}
