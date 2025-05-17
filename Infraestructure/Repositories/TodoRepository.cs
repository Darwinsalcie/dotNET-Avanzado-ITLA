using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repositories
{
    public class TodoRepository : IGenericRepository<Todo>
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

        // Método para obtener un elemento por su Id
        public async Task<Todo> GetByIdAsync(int id)
            => await _context.Todos.FirstOrDefaultAsync(x => x.Id == id);


        public async Task<(bool IsSucces, string Message)> AddAsync(Todo entity)
        {
            try
            {
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
    }

}
