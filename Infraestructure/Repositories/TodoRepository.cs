using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infraestructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        // Inyectamos el contexto de la base de datos
        private readonly AppDbContext _context;
        
        public TodoRepository(AppDbContext context)
        {
            _context = context;
        }

        // Método para obtener todos los elementos de la tabla Todos
        public async Task<IEnumerable<Todo>> GetAllAsync()
            => await _context.Todos.ToListAsync();

        public async Task<IEnumerable<Todo>> GetByStatusAsync(int status) 
        {
            return await _context.Todos
                .Where(x => x.Status == (Status)status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Todo>> GetByPriorityAsync(int priority)
        {
            return await _context.Todos
                .Where(x => x.Priority == (Priority)priority)
                .ToListAsync();
        }

        public async Task<IEnumerable<Todo>> GetByTitleAsync(string title)
        {
            return await _context.Todos
                .Where(x => x.Title.Contains(title))
                .ToListAsync();
        }

        public async Task<IEnumerable<Todo>> filterTodoAsync(int? status, int? priority, string? title, DateTime? dueDate) 
        {
            // Al usar _context.Todos.AsQueryable(), se obtiene un objeto IQueryable que permite construir consultas de manera dinámica.
            // Cada vez que se agrega un filtro con .Where(), no se ejecuta la consulta inmediatamente, sino que se va construyendo una expresión.
            // Entity Framework Core traduce toda la expresión LINQ a una sola consulta SQL cuando se llama a ToListAsync().
            // Por ejemplo, si se agregan varios .Where(), el SQL generado incluirá todas las condiciones en la cláusula WHERE.
            // Esto significa que solo se traen de la base de datos los registros que cumplen con los filtros, y no toda la tabla.
            // En resumen: .Where() en LINQ se traduce a WHERE en SQL, y ToListAsync() ejecuta el SELECT final con todos los filtros aplicados.
            var query = _context.Todos.AsQueryable();
            if (status.HasValue)
            {
                query = query.Where(x => x.Status == (Status)status.Value);
            }
            if (priority.HasValue)
            {
                query = query.Where(x => x.Priority == (Priority)priority.Value);
            }
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(x => x.Title.Contains(title));
            }
            if (dueDate.HasValue)
            {
                query = query.Where(x => x.DueDate.HasValue && x.DueDate.Value.Date == dueDate.Value.Date);
            }
            return await query.ToListAsync();
        }

        // Método para obtener un elemento por su Id
        public async Task<Todo> GetByIdAsync(int id)
            => await _context.Todos.FirstOrDefaultAsync(x => x.Id == id);


        // CRUDS
        public async Task<(bool IsSucces, string Message)> AddAsync(Todo entity)
        {
            try
            {
                // Verificamos si el elemento ya existe en la base de datos
                var exists =  _context.Todos.Any(x => 
                
                x.Title == entity.Title

                );

                if (exists) 
                {
                    return (false, "Ya existe una tarea con ese titulo");
                }

                await _context.Todos.AddAsync(entity);
                await _context.SaveChangesAsync();
                return (true, "Tarea agregado correctamente.");
            }

            catch (Exception ex) 
            {
                return (false, "Error al agregar el elemento a la base de datos.");
                throw new Exception(ex + ": Error al agregar el elemento a la base de datos." );
            }
            
        }
        public async Task<(bool IsSucces, string Message)> UpdateAsync(Todo entity)
        {
            try
            {
                _context.Todos.Update(entity);
                await _context.SaveChangesAsync();
                return (true, "Tarea actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return (false, "Error al actualizar el elemento en la base de datos.");
                // Manejo de excepciones
                throw new Exception(ex + ": Error al actualizar el elemento en la base de datos.");
            }
        }
        public async Task<(bool IsSucces, string Message)> DeleteAsync(int id)
        {
            try
            {
                // Buscamos el elemento por su Id para ver si existe
                var todo =  await _context.Todos.FindAsync(id);

                // Si existe, lo eliminamos
                if (todo != null)
                {
                    _context.Todos.Remove(todo);
                    await _context.SaveChangesAsync();
                    return (true, "Tarea eliminado correctamente.");

                }
                else
                {
                    return (false, "No se encontró el elemento a eliminar.");
                    // Si no existe, lanzamos una excepción
                    throw new Exception("No se encontró el elemento a eliminar.");
                }
            }
            catch (Exception ex)
            {
                return (false, "Error al eliminar el elemento de la base de datos.");
                // Manejo de excepciones
                throw new Exception(ex + ": Error al eliminar el elemento de la base de datos.");
            }
        }


        //Métodos adicionales para contar tareas completadas y pendientes
        public async Task<double> ContarTareasCompletadasAsync()
        {
            // Conteo total de tareas
            int totalCount = await _context.Todos.CountAsync();

            if (totalCount == 0)
                return 0.0;

            // Conteo de tareas completadas
            int completedCount = await _context.Todos
                .CountAsync(t => t.Status == Status.Completado);

            return (double)completedCount * 100.0 / totalCount;
        }

        public async Task<double> ContarTareasPendientesAsync()
        {
            // Conteo total de tareas
            int totalCount = await _context.Todos.CountAsync();
            if (totalCount == 0)
                return 0.0;
            // Conteo de tareas pendientes
            int pendingCount = await _context.Todos
                .CountAsync(t => t.Status == Status.Pendiente);
            return (double)pendingCount * 100.0 / totalCount;
        }
    }

}
