using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.Context;
using Microsoft.EntityFrameworkCore;


namespace Infraestructure.Repositories
{
    public class TodoRepository<T> : ITodoRepository<T>
    {
        private readonly AppDbContext _db;

        public TodoRepository(AppDbContext db)
        {
            _db = db;
        }

        private DbSet<Todo<T>> Set => _db.Set<Todo<T>>();

        public async Task<IEnumerable<Todo<T>>> GetAllAsync()
        {
            return await Set.AsNoTracking().ToListAsync();
        }

        public async Task<Todo<T>?> GetByIdAsync(int id)
        {
            return await Set.FindAsync(id);
        }

        public async Task AddAsync(Todo<T> todo)
        {
            await Set.AddAsync(todo);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Todo<T> todo)
        {
            Set.Update(todo);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var todo = await Set.FindAsync(id);
            if (todo != null)
            {
                Set.Remove(todo);
                await _db.SaveChangesAsync();
            }
        }
    }

}
