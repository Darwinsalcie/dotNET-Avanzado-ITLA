using Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Si no se quiere hacer override la forma sencilla de hacerlo es:
        public DbSet<Todo> Todos { get; set; }

        // Para cambiar el nombre de la tabla se haría un override de OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cambiamos el nombre de la tabla a "TodoList"
            modelBuilder.Entity<Todo>().ToTable("Todo");
            // Otras configuraciones de la base de datos
        }


    }

}
