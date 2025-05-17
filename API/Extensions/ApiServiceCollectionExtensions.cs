using Application.Interfaces;
using Application.Services;
using Domain.Delegates;
using Domain.Entities;
using Domain.Interfaces;
using Infraestructure.Repositories;

namespace API.Extensions
{
    public static class ApiServiceCollectionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {

            //// Registrar repositorio (ajusta según tu implementación)
            //services.AddScoped<ITodoRepository<string>, TodoRepository<string>>();

            // Validación (Scoped)
            services.AddScoped<EntityValidator<Todo>>(_ =>
                todo => !string.IsNullOrWhiteSpace(todo.Title)
                        && (!todo.DueDate.HasValue || todo.DueDate > DateTime.UtcNow)
            );

            // Notificación (Scoped)
            services.AddScoped<Action<Todo>>(_ =>
                todo => Console.WriteLine($"[Notification] Todo #{todo.Id} – {todo.Title}")
            );

            // Cálculo de días restantes (Scoped)
            services.AddScoped<Func<Todo, int>>(_ =>
                todo =>
                {
                    if (!todo.DueDate.HasValue) return -1;
                    var remaining = todo.DueDate.Value - DateTime.UtcNow;
                    return (int)Math.Ceiling(remaining.TotalDays);
                }
            );



            services.AddScoped<ITodoService, TodoService>();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }
    }
}
